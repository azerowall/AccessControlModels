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
    /// Логика взаимодействия для Explorer.xaml
    /// </summary>
    public partial class Explorer : Page
    {
        ViewModels.ExplorerVM model;
        public Explorer()
        {
            InitializeComponent();
            model = new ViewModels.ExplorerVM();
            DataContext = model;
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox list = (ListBox)sender;
            if (list.SelectedValue != null)
                if (model.OpenFileCommand.CanExecute(list.SelectedValue))
                    model.OpenFileCommand.Execute(list.SelectedValue);
            // параметр команды не используется в настоящее время
        }
    }
}
