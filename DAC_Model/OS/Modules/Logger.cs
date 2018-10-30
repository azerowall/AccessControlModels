using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DAC_Model.OS
{
    class Logger
    {
        static readonly string datafile = "log.os";
        Core core;
        StreamWriter log;

        public enum LogType
        {
            Info, Warning, Error
        }
        
        public Logger(Core core)
        {
            this.core = core;

            var path = Path.Combine(core.Root, FileSystem.SysDir, datafile);
            log = new StreamWriter(File.OpenWrite(path));
        }

        public void Uninit()
        {
            log.Close();
        }

        public void Info(string msg) => Log(LogType.Info, msg);
        public void Warning(string msg) => Log(LogType.Warning, msg);
        public void Error(string msg) => Log(LogType.Error, msg);

        public void Log(LogType type, string msg)
        {
            log.WriteLine($"[{DateTime.Now}] {type} - {msg}");
        }
    }
}
