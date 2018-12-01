using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DAC_Model.OS
{
    class SessionsMgr
    {
        public static readonly string SessionsDir = "sessions";

        Core core;

        public SessionsMgr(Core core)
        {
            this.core = core;

            if (!Directory.Exists(Path.Combine(core.Root, SessionsDir)))
                Directory.CreateDirectory(Path.Combine(core.Root, SessionsDir));
        }

        public void Uninit()
        {

        }

        public string GetSessionId(UserSubject user, UserRole role) => $"{user.Id}-{role.Id}";

        public bool HasSession(string ssid) =>
            Directory.Exists(Path.Combine(core.Root, SessionsDir, ssid));

        public void StartSession(string ssid)
        {
            if (HasSession(ssid))
                CloseSession(ssid);
            Directory.CreateDirectory(Path.Combine(core.Root, SessionsDir, ssid));
        }

        public void CloseSession(string ssid)
        {
            var sessDir = Path.Combine(core.Root, SessionsDir, ssid);
            var workDir = core.Root;
            string[] files = Directory.GetFiles(sessDir);
            foreach (var file in files)
            {

                var dstFile = Path.Combine(workDir, Path.GetFileName(file));

                if (File.Exists(dstFile))
                    File.Delete(dstFile);

                File.Move(file, dstFile);
                File.Delete(file);
            }
            Directory.Delete(sessDir);
        }

        public string GetPath(string ssid, string filename) =>
            Path.Combine(core.Root, SessionsDir, ssid, filename);
    }
}
