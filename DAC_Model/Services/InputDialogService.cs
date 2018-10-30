using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC_Model
{
    static class InputDialogService
    {
        public static string Show(string title, string message)
        {
            var wnd = new Windows.InputDialogBox(title, message);
            wnd.ShowDialog();
            return wnd.Result;
        }
    }
}
