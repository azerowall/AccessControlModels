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
    /// Логика взаимодействия для InputDialogBox.xaml
    /// </summary>
    public partial class InputDialogBox : Window
    {
        public InputDialogBox(string title, string message)
        {
            InitializeComponent();
            Title = title;
            tbMessage.Text = message;
            Result = null;
        }
        public string Result { get; private set; }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = tbInput.Text;
            Close();
        }

        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Result = tbInput.Text;
                Close();
            }
            else if (e.Key == Key.Escape)
            {
                Result = null;
                Close();
            }
        }
    }
}
