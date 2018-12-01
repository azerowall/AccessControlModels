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
    /// Логика взаимодействия для SelectRole.xaml
    /// </summary>
    public partial class SelectRole : Page
    {
        ViewModels.SelectRoleVM model;

        public SelectRole()
        {
            InitializeComponent();
            DataContext = model = new ViewModels.SelectRoleVM();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            if (lb.SelectedIndex != -1 && model.SelectRoleCommand.CanExecute(null))
                model.SelectRoleCommand.Execute(null);
        }
    }
}
