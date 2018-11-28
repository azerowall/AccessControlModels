using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC_Model.OS
{
    class OsException : Exception
    {
        public OsException(string message) : base(message)
        {

        }
    }
}
