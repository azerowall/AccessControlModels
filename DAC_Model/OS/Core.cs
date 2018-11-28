using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAC_Model.OS
{
    class Core
    {
        public string Root;
        public RefMonitor RMon;

        public FileSystem Fs;
        public UserMgr Umgr;
        public Logger Log;
        
        public UserSubject CurrentUser;
        
        public Core(string root)
        {
            Root = root;
            Fs = new FileSystem(this);
            Log = new Logger(this);
            RMon = new RefMonitor(this);
            Umgr = new UserMgr(this);
        }

        public void Exit()
        {
            Umgr.Uninit();
            RMon.Uninit();
            Log.Uninit();
            Fs.Uninit();
        }

        public void Login(string user, string password)
        {
            CurrentUser = Umgr.Login(user, password);
        }
        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
