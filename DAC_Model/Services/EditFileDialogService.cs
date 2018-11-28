using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC_Model
{
    static class EditFileDialogService
    {
        public static string Show(string title, string content)
        {
            var wnd = new Windows.EditFileDialog(title, content);
            wnd.ShowDialog();
            return wnd.ContentText;
        }
    }
}
