using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using Newtonsoft.Json;

namespace DAC_Model.OS
{
    class FileObject : IComparable
    {
        public int Id;
        public string Path;

        public FileObject(int id, string path)
        {
            Id = id;
            Path = path;
        }

        public override string ToString() => Path;

        public int CompareTo(object o)
        {
            var file = (FileObject)o;
            return Id - file.Id;
        }
    }

    class FileSystem
    {
        static readonly string datafile = "files.os";
        public static readonly string SysDir = "sys";
        Core core;

        public ObservableCollection<FileObject> Files
        {
            get;set;
        }

        public FileSystem(Core core)
        {
            this.core = core;
            Files = new ObservableCollection<FileObject>();
            InitFS();
        }

        private void InitFS()
        {
            if (!Directory.Exists(core.Root))
                Directory.CreateDirectory(core.Root);
            if (!Directory.Exists(Path.Combine(core.Root, SysDir)))
                Directory.CreateDirectory(Path.Combine(core.Root, SysDir));

            var path = GetSysPath(datafile);
            if (!File.Exists(path))
                File.Create(path).Close();

            Files = JsonConvert.DeserializeObject<ObservableCollection<FileObject>>(File.ReadAllText(path));
            Files = Files != null ? Files : new ObservableCollection<FileObject>();
        }
        public void Uninit()
        {
            File.WriteAllText(core.Fs.GetSysPath(datafile),
                JsonConvert.SerializeObject(Files));
        }

        public string GetSysPath(string filename)
        {
            return Path.Combine(core.Root, SysDir, filename);
        }

        string GetPath(string filename)
        {
            string path = core.Sessions.GetPath(core.SessionId, filename);
            if (!File.Exists(path))
            {
                path = Path.Combine(core.Root, filename);
            }
            return path;
        }

        public FileObject GetFileObject(string path)
        {
            try
            {
                return Files.First(f => f.Path == path);
            }
            catch (InvalidOperationException)
            {
                throw new FileSystemException($"Файла {path} не существует");
            }
        }

        public void Write(FileObject file, string data)
        {
            core.CheckAccess(file, AccessRights.Write);

            string path = core.Sessions.GetPath(core.SessionId, file.Path);
            if (!File.Exists(path))
            {
                // при изменении файла добавляем его в сессию, если он еще не там
                File.Copy(Path.Combine(core.Root, file.Path), path);
            }

            File.WriteAllText(path, data);

            core.Log.Info($"{core.CurrentUser} изменил файл {file}");
        }

        public string Read(FileObject file)
        {
            core.CheckAccess(file, AccessRights.Read);
            core.Log.Info($"{core.CurrentUser} прочитал файл {file}");
            if (!File.Exists(GetPath(file.Path)))
                return string.Empty;
            return File.ReadAllText(GetPath(file.Path));
        }

        public FileObject Create(string path)
        {
            CheckFileName(path);

            int id = Files.Count > 0 ? Files[Files.Count - 1].Id + 1 : 0;

            Files.Add(new FileObject(id, path));
            var file = Files[Files.Count - 1];

            // фулл права для владельца файла
            core.RMon.SetRights(core.CurrentUserRole, AccessRights.Full, file);

            File.Create(core.Sessions.GetPath(core.SessionId, file.Path)).Close();

            core.Log.Info($"{core.CurrentUser} создал файл {file}");

            return file;
        }

        // не переделан под сессии
        public void Remove(FileObject file)
        {
            core.CheckAccess(file, AccessRights.Write);
            File.Delete(Path.Combine(core.Root, file.Path));
            Files.Remove(file);
            core.Log.Info($"{core.CurrentUser} удалил файл {file}");
        }

        // не переделан под сессии
        public void Rename(FileObject file, string newName)
        {
            core.CheckAccess(file, AccessRights.Write);
            CheckFileName(newName);

            File.Move(Path.Combine(core.Root, file.Path), Path.Combine(core.Root, newName));

            // простой установки имени не происходит
            // т.к. нужно, чтобы коллекция заметила изменения
            file.Path = newName;
            int i = Files.IndexOf(file);
            Files[i] = null;
            Files[i] = file;
        }

        private void CheckFileName(string path)
        {
            if (Files.Any(f => f.Path == path))
                throw new FileSystemException($"Файл - \"{path}\" уже существует");
            if (!Regex.IsMatch(path, @"^[a-zа-я0-9 \._-]+$", RegexOptions.IgnoreCase))
                throw new FileSystemException($"Файл - \"{path}\" содержит зарпещенные символы");
        }
    }

    class FileSystemException : OsException
    {
        public FileSystemException(string message) : base(message) { }
    }
}
