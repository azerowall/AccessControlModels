using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC_Model
{
    static class OsService
    {
        static OS.Core os;
        public static OS.Core Os
        {
            get
            {
                if (os == null)
                    os = new OS.Core("root");
                return os;
            }
        }
        public static OS.Core GetOS() => Os;
    }
}
