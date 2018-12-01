using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

using Newtonsoft.Json;

namespace DAC_Model.OS
{
    public enum UserType { User, Admin }

    class UserSubject : IComparable
    {
        public int Id;
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
            var path = core.Fs.GetSysPath(datafile);
            if (!File.Exists(path))
                File.Create(path).Close();

            Users = JsonConvert.DeserializeObject<List<UserSubject>>(File.ReadAllText(path));
            Users = Users != null ? Users : new List<UserSubject>();
        }
        public void Uninit()
        {
            File.WriteAllText(core.Fs.GetSysPath(datafile), JsonConvert.SerializeObject(Users));
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

            int id = Users.Count > 0 ? Users[Users.Count - 1].Id + 1 : 0;

            var user = new UserSubject(id, name, password, type);
            Users.Add(user);

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

    class UserMgrException : OsException
    {
        public UserMgrException(string message) : base(message) { }
    }
}
