using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace DAC_Model.OS
{
    class FileObject : IOsObject, IComparable
    {
        public int Id { get; set; }
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

        //List<FileObject> files;
        int idCounter;

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
            try
            {
                LoadFS();
            }
            catch (Exception e)
            {
                if (!Directory.Exists(core.Root))
                    Directory.CreateDirectory(core.Root);
                string sysdir = Path.Combine(core.Root, SysDir);
                if (!Directory.Exists(sysdir))
                    Directory.CreateDirectory(sysdir);
                SysWrite(datafile, "");
                idCounter = 0;
            }
        }
        public void Uninit()
        {
            SaveFS();
        }

        private void LoadFS()
        {
            string data = SysRead(datafile);
            foreach (var line in data.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var words = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Files.Add(new FileObject(int.Parse(words[0]), words[1]));
            }
            idCounter = (Files.Count > 0) ? Files[Files.Count - 1].Id + 1 : 0;
        }

        private void SaveFS()
        {
            var sb = new StringBuilder();
            foreach (var file in Files)
                sb.AppendLine($"{file.Id} {file.Path}");
            SysWrite(datafile, sb.ToString());
        }

        // работа с системными файлами - проверки прав нет,
        // т.к. эти функции не предназначены для пользователя
        public string SysRead(string filename)
        {
            return File.ReadAllText(Path.Combine(core.Root, SysDir, filename));
        }
        public void SysWrite(string filename, string data)
        {
            File.WriteAllText(Path.Combine(core.Root, SysDir, filename), data);
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
            core.RMon.Check(core.CurrentUser, AccessRights.Write, file);
            core.Log.Info($"{core.CurrentUser} изменил файл {file}");
            File.WriteAllText(Path.Combine(core.Root, file.Path), data);
        }

        public string Read(FileObject file)
        {
            core.RMon.Check(core.CurrentUser, AccessRights.Read, file);
            core.Log.Info($"{core.CurrentUser} прочитал файл {file}");
            return File.ReadAllText(Path.Combine(core.Root, file.Path));
        }

        public FileObject Create(string path)
        {
            CheckFileName(path);

            Files.Add(new FileObject(idCounter++, path));
            var file = Files[Files.Count - 1];

            // задаем все права владельцу
            core.RMon.Set(core.CurrentUser, AccessRights.Full, file);

            // задаем права для всех админов в системе
            var admins = core.Umgr.Users.Where(u => u.Type == UserType.Admin);
            foreach (var admin in admins)
                core.RMon.Set(admin, AccessRights.Full, file);

            File.Create(Path.Combine(core.Root, path)).Close();

            core.Log.Info($"{core.CurrentUser} создал файл {file}");

            return file;
        }

        public void Remove(FileObject file)
        {
            core.RMon.Check(core.CurrentUser, AccessRights.Remove, file);
            File.Delete(Path.Combine(core.Root, file.Path));
            Files.Remove(file);
            core.Log.Info($"{core.CurrentUser} удалил файл {file}");
        }

        public void Rename(FileObject file, string newName)
        {
            core.RMon.Check(core.CurrentUser, AccessRights.Write, file);
            CheckFileName(newName);

            File.Move(Path.Combine(core.Root, file.Path), Path.Combine(core.Root, newName));

            // простой установки имени не происходит
            // т.к. нужно, чтобы коллекция заметила изменения
            int i = Files.IndexOf(file);
            Files[i] = new FileObject(file.Id, newName);
        }

        private void CheckFileName(string path)
        {
            if (Files.Any(f => f.Path == path))
                throw new FileSystemException($"Файл - \"{path}\" уже существует");
            if (!Regex.IsMatch(path, @"^[a-zа-я0-9\._-]+$", RegexOptions.IgnoreCase))
                throw new FileSystemException($"Файл - \"{path}\" содержит зарпещенные символы");
        }
    }

    class FileSystemException : Exception
    {
        public FileSystemException(string message) : base(message) { }
    }
}
