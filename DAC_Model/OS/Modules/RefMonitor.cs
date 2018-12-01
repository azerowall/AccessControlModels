using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace DAC_Model.OS
{
    [Flags]
    enum AccessRights
    {
        None = 0,
        Read = 0x01,
        Write = 0x02,
        //Remove = 0x04,
        Full = Read | Write //| Remove
    }


    class UserRole : IComparable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UserRole(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;

        public int CompareTo(object o)
        {
            var r = (UserRole)o;
            return Id - r.Id;
        }
    }

    class RefMonitor
    {
        static readonly string rolesfile = "roles.os";
        static readonly string usersrolesfile = "usersroles.os";
        static readonly string matrixfile = "accessmatrix.os";
        Core core;

        // список существующих ролей
        public SortedList<int, UserRole> Roles;
        // список ролей для каждого пользователя
        public Dictionary<int, HashSet<int>> UsersRoles;                        // [uid] => {roles}
        // матрица доступа ролей к объектам
        public Dictionary<int, Dictionary<int, AccessRights>> AccessMatrix;  // [rid, fid] => rights

        public RefMonitor(Core core)
        {
            this.core = core;

            Roles = new SortedList<int, UserRole>();
            UsersRoles = new Dictionary<int, HashSet<int>>();
            AccessMatrix = new Dictionary<int, Dictionary<int, AccessRights>>();

            Init();
            //InitRoles();
            //InitAccessMatrix();
        }

        private void Init()
        {
            var path = core.Fs.GetSysPath(rolesfile);
            if (!File.Exists(path))
                File.Create(path).Close();
            Roles = JsonConvert.DeserializeObject<SortedList<int, UserRole>>(File.ReadAllText(path));
            Roles = Roles != null ? Roles : new SortedList<int, UserRole>();

            path = core.Fs.GetSysPath(usersrolesfile);
            if (!File.Exists(path))
                File.Create(path).Close();
            UsersRoles = JsonConvert.DeserializeObject<Dictionary<int, HashSet<int>>>(File.ReadAllText(path));
            UsersRoles = UsersRoles != null ? UsersRoles : new Dictionary<int, HashSet<int>>();

            path = core.Fs.GetSysPath(matrixfile);
            if (!File.Exists(path))
                File.Create(path).Close();
            AccessMatrix = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, AccessRights>>>(File.ReadAllText(path));
            AccessMatrix = AccessMatrix != null ? AccessMatrix : new Dictionary<int, Dictionary<int, AccessRights>>();
        }

        public void Uninit()
        {
            var path = core.Fs.GetSysPath(rolesfile);
            File.WriteAllText(path, JsonConvert.SerializeObject(Roles));

            path = core.Fs.GetSysPath(usersrolesfile);
            File.WriteAllText(path, JsonConvert.SerializeObject(UsersRoles));

            path = core.Fs.GetSysPath(matrixfile);
            File.WriteAllText(path, JsonConvert.SerializeObject(AccessMatrix));
        }


        public string RightsToString(AccessRights rights)
        {
            var sb = new StringBuilder();
            if (rights.HasFlag(AccessRights.Read)) sb.Append('r');
            if (rights.HasFlag(AccessRights.Write)) sb.Append('w');
            //if (rights.HasFlag(AccessRights.Remove)) sb.Append('d');
            return sb.ToString();
        }
        public AccessRights ParseRights(string str)
        {
            AccessRights rights = AccessRights.None;
            foreach (char ch in str)
            {
                if (ch == 'r') rights |= AccessRights.Read;
                else if (ch == 'w') rights |= AccessRights.Write;
                //else if (ch == 'd') rights |= AccessRights.Remove;
                else
                    throw new OsException("Неверное строковое представление прав доступа");
            }
            return rights;
        }


        public IEnumerable<UserRole> GetUserRoles(int uid)
        {
            if (UsersRoles.ContainsKey(uid))
                return UsersRoles[uid].Select(rid => Roles[rid]);
            return new UserRole[0];
        }


        public AccessRights Get(UserRole sub, FileObject obj)
        {
            if (sub == null)
                return AccessRights.None;
            if (AccessMatrix.ContainsKey(sub.Id) && AccessMatrix[sub.Id].ContainsKey(obj.Id))
                return AccessMatrix[sub.Id][obj.Id];
            else
                return AccessRights.None;
        }
        public bool Can(UserRole sub, AccessRights rights, FileObject obj)
        {
            return Get(sub, obj).HasFlag(rights);
        }
        public void Check(UserRole sub, AccessRights rights, FileObject obj)
        {
            if (!Can(sub, rights, obj))
                throw new HasNoRightsException(~Get(sub, obj) & rights, obj);
        }

        public UserRole AddRole(string name)
        {
            int rid = Roles.Count > 0 ? Roles[Roles.Count - 1].Id + 1 : 0;
            Roles.Add(rid, new UserRole(rid, name));
            return Roles[Roles.Count - 1];
        }
        public void RemoveRole(UserRole role)
        {
            Roles.Remove(role.Id);
            foreach (var uid in UsersRoles.Keys)
                UsersRoles[uid].Remove(role.Id);
            AccessMatrix.Remove(role.Id);
        }
        
        public void AddRoleToUser(UserSubject sub, UserRole role)
        {
            if (!UsersRoles.ContainsKey(sub.Id))
                UsersRoles.Add(sub.Id, new HashSet<int>() { role.Id });
            else
                UsersRoles[sub.Id].Add(role.Id);
        }
        public void RemoveRoleFromUser(UserSubject sub, UserRole role)
        {
            if (UsersRoles.ContainsKey(sub.Id))
                UsersRoles[sub.Id].Remove(role.Id);
        }

        public void SetRights(UserRole role, AccessRights rights, FileObject obj)
        {
            if (!AccessMatrix.ContainsKey(role.Id))
                AccessMatrix.Add(role.Id, new Dictionary<int, AccessRights>() { { obj.Id, rights } });
            else if (!AccessMatrix[role.Id].ContainsKey(obj.Id))
                AccessMatrix[role.Id].Add(obj.Id, rights);
            else
                AccessMatrix[role.Id][obj.Id] = rights;
        }

        //public void Remove(UserSubject sub) => SubjectsLevels.Remove(sub.Id);
        //public void Remove(FileObject obj) => ObjectsLevels.Remove(obj.Id);
    }

    class HasNoRightsException : OsException
    {
        public FileObject Object;
        public AccessRights NeededRights;
        public HasNoRightsException(AccessRights rights, FileObject obj) :
            base($"Для доступа к \"{obj.ToString()}\" недостаточно следующих прав: {rights.ToString()}")
        {
            Object = obj;
            NeededRights = rights;
        }
    }
}
