using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace DAC_Model.OS
{
    class Core
    {
        public string Root;
        public RefMonitor RMon;

        public FileSystem Fs;
        public UserMgr Umgr;
        public Logger Log;
        public SessionsMgr Sessions;
        
        public UserSubject CurrentUser;
        public UserRole CurrentUserRole;
        public string SessionId;
        
        public Core(string root)
        {
            Root = root;
            Fs = new FileSystem(this);
            Log = new Logger(this);
            Umgr = new UserMgr(this);
            RMon = new RefMonitor(this);
            Sessions = new SessionsMgr(this);

            if (Umgr.Users.Count == 0)
            {
                var adm = Umgr.Registrate("admin", "roottoor", UserType.Admin);
            }
            if (RMon.Roles.Count == 0)
            {
                var admr = RMon.AddRole("Администратор");
                RMon.AddRole("Пользователь");
                RMon.AddRoleToUser(Umgr.GetUserSubject("admin"), admr);
            }

            // изменить все на json
            // проверить парсинг dictionary<int, ...> при указании типа
            // в объекте юзера сохранять последнюю выбранную им роль
        }

        public void Exit()
        {
            RMon.Uninit();
            Umgr.Uninit();
            Log.Uninit();
            Fs.Uninit();
        }

        public IEnumerable<UserRole> GetUserRoles() =>
            RMon.GetUserRoles(CurrentUser.Id);

        // ============= Sessions

        //public bool HasSavedSession() => Sessions.HasSession(SessionId);
        //public void RecoverySession() => Sessions.
        //void SaveSession() => CurrentUser.RoleId = CurrentUserRole.Id;
        //void RemoveSession() => CurrentUser.RoleId = -1;


        // ============= End of Sessions


        public void Login(string user, string password)
        {
            CurrentUser = Umgr.Login(user, password);
        }
        // должна быть вызвана сразу после логина
        public void SelectRole(UserRole role)
        {
            CurrentUserRole = role;
            SessionId = Sessions.GetSessionId(CurrentUser, CurrentUserRole);
        }

        public void Logout(bool closeSession = true)
        {
            if (closeSession)
                Sessions.CloseSession(SessionId);
            CurrentUser = null;
        }

        public bool HasAccess(FileObject file, AccessRights rights)
        {
            //if (CurrentUser.Type == UserType.Admin) return true;
            return RMon.Can(CurrentUserRole, rights, file);
        }

        public void CheckAccess(FileObject file, AccessRights rights)
        {
            //if (CurrentUser.Type != UserType.Admin)
                RMon.Check(CurrentUserRole, rights, file);
        }
    }
}
