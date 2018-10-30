using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace DAC_Model.OS
{
    public enum UserType { User, Admin }

    class UserSubject : IOsSubject, IComparable
    {
        public int Id { get; set; }
        public string Name;
        public string Password;
        public UserType Type;

        public UserSubject(int id, string name, string password, UserType type)
        {
            Id = id;
            Name = name;
            Password = password;
            Type = type;
        }

        public override string ToString() => Name;

        public int CompareTo(object o)
        {
            var user = (UserSubject)o;
            return Id - user.Id;
        }
    }

    class UserMgr
    {
        static readonly string datafile = "users.os";

        Core core;
        int uidCounter;


        public List<UserSubject> Users
        {
            get; set;
        }

        public UserMgr(Core core)
        {
            this.core = core;
            Users = new List<UserSubject>();
            InitUsers();
        }

        private void InitUsers()
        {
            try
            {
                LoadUsers();
            }
            catch (FileNotFoundException)
            {
                uidCounter = 0;
                Registrate("admin", "roottoor", UserType.Admin);
            }
        }
        private void LoadUsers()
        {
            var data = core.Fs.SysRead(datafile);
            foreach (var line in data.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var words = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Users.Add(new UserSubject(int.Parse(words[0]), words[1],
                                            words[2], (UserType)int.Parse(words[3])));
            }
            uidCounter = Users.Count > 0 ? Users[Users.Count - 1].Id + 1 : 0;
        }
        private void SaveUsers()
        {
            var sb = new StringBuilder();
            foreach (var user in Users)
                sb.AppendLine($"{user.Id} {user.Name} {user.Password} {(int)user.Type}");
            core.Fs.SysWrite(datafile, sb.ToString());
        }
        public void Uninit()
        {
            SaveUsers();
        }

        public UserSubject GetUserSubject(string name)
        {
            try
            {
                return Users.First(u => u.Name == name);
            }
            catch (InvalidOperationException)
            {
                throw new UserMgrException($"Пользователя {name} не существует");
            }
        }

        public UserSubject Registrate(string name, string password, UserType type = UserType.User)
        {
            if (Users.Any(u => u.Name == name))
                throw new UserMgrException($"Пользователь с именем {name} уже существует");
            if (!Regex.IsMatch(name, @"^[a-zа-я][a-zа-я0-9]{3,20}$", RegexOptions.IgnoreCase))
                throw new UserMgrException($"Имя пользователя {name} имеет неверный формат");
            if (!Regex.IsMatch(password, @"^[a-zA-Z0-9]{5,30}$"))
                throw new UserMgrException("Пароль имеет неверный формат");

            var user = new UserSubject(uidCounter++, name, password, type);
            Users.Add(user);

            // если админ, то устанавливаем права для всех файлов
            if (user.Type == UserType.Admin)
                foreach (var file in core.Fs.Files)
                    core.RMon.Set(user, AccessRights.Full, file);

            return user;
        }

        public UserSubject Login(string name, string password)
        {
            var user = GetUserSubject(name);
            CheckPassword(user, password);
            return user;
        }

        public void CheckPassword(UserSubject user, string password)
        {
            if (user.Password != password) // пока и так сойдет
                throw new UserMgrException("Неверный пароль");
        }

        public void Remove(UserSubject user)
        {
            Users.Remove(user);
        }
    }

    class UserMgrException : Exception
    {
        public UserMgrException(string message) : base(message) { }
    }
}
