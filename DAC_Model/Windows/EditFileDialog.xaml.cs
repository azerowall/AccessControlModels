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
using System.Windows.Shapes;

namespace DAC_Model.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditFileDialog.xaml
    /// </summary>
    public partial class EditFileDialog : Window
    {
        public string ContentText;

        public EditFileDialog(string title, string content)
        {
            InitializeComponent();
            Title = title;
            tbContent.Text = content;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ContentText = tbContent.Text;
            Close();
        }
    }
}
