using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DAC_Model.Pages
{
    /// <summary>
    /// Логика взаимодействия для AccessTable.xaml
    /// </summary>
    public partial class AccessTable : Page
    {
        ViewModels.AccessTableVM model;

        public AccessTable()
        {
            InitializeComponent();
            DataContext = model = new ViewModels.AccessTableVM();
            UpdateAccessMatrix();

            model.PropertyChanged += Model_PropertyChanged;
            DataContextChanged += AccessTable_DataContextChanged;
        }

        /*
            Это работает только потому, что навигатор обновляет датаконтекст
            Это костыль, который работает благодаря другому костылю
        */
        private void AccessTable_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateAccessMatrix();
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Roles")
                UpdateAccessMatrix();
        }

        void UpdateAccessMatrix()
        {
            var os = OsService.GetOS();

            Grid gMatrix = new Grid();

            gMatrix.VerticalAlignment = VerticalAlignment.Top;
            gMatrix.HorizontalAlignment = HorizontalAlignment.Left;

            for (int i = 0; i < os.RMon.Roles.Count + 1; i++)
                gMatrix.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < os.Fs.Files.Count + 1; i++)
                gMatrix.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < os.Fs.Files.Count; i++)
            {
                var tbHeader = new TextBlock();
                tbHeader.Text = os.Fs.Files[i].ToString();
                tbHeader.VerticalAlignment = VerticalAlignment.Center;
                tbHeader.TextAlignment = TextAlignment.Center;
                tbHeader.TextWrapping = TextWrapping.Wrap;
                tbHeader.Margin = new Thickness(3);

                gMatrix.Children.Add(tbHeader);
                Grid.SetRow(tbHeader, 0);
                Grid.SetColumn(tbHeader, i + 1);
            }
            int irole = 0;
            foreach (var role in os.RMon.Roles.Values)
            {
                var tbHeader = new TextBlock();
                tbHeader.Text = role.ToString();
                tbHeader.VerticalAlignment = VerticalAlignment.Center;
                tbHeader.TextAlignment = TextAlignment.Center;
                tbHeader.TextWrapping = TextWrapping.Wrap;
                tbHeader.Margin = new Thickness(3);

                gMatrix.Children.Add(tbHeader);
                Grid.SetRow(tbHeader, irole + 1);
                Grid.SetColumn(tbHeader, 0);
                irole += 1;
            }

            irole = 0;
            foreach (var role in os.RMon.Roles.Values)
            {
                for (int i = 0; i < os.Fs.Files.Count; i++)
                {
                    var tbRights = new TextBox();
                    tbRights.Tag = new object[] { role, os.Fs.Files[i] };
                    var rights = os.RMon.Get(role, os.Fs.Files[i]);
                    tbRights.Text = os.RMon.RightsToString(rights);
                    tbRights.TextChanged += AccessMatrix_TextBox_TextChanged;

                    gMatrix.Children.Add(tbRights);
                    Grid.SetRow(tbRights, irole + 1);
                    Grid.SetColumn(tbRights, i + 1);
                }
                irole += 1;
            }
            tiAccessMatrix.Content = gMatrix;
            //gAccessMatrix = gMatrix;
        }

        private void AccessMatrix_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox tb = (TextBox)sender;
                OS.UserRole role = (OS.UserRole)((object[])tb.Tag)[0];
                OS.FileObject file = (OS.FileObject)((object[])tb.Tag)[1];
                var rights = OsService.GetOS().RMon.ParseRights(tb.Text);
                OsService.GetOS().RMon.SetRights(role, rights, file);
            }
            catch (OS.OsException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lbRoles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (model.RemoveRoleCommand.CanExecute(null))
                model.RemoveRoleCommand.Execute(null);
        }
    }
}
