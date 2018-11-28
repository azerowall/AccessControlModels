using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DAC_Model.OS
{
    [Flags]
    enum AccessRights
    {
        None = 0,
        Read = 0x01,
        Write = 0x02,
        Remove = 0x04,
        Full = Read | Write | Remove
    }

    class RefMonitor
    {
        //static readonly string datafile = "rights.os";
        Core core;

        public RefMonitor(Core core)
        {
            this.core = core;
            /*SubjectsLevels = new SortedList<int, int>();
            ObjectsLevels = new SortedList<int, int>();
            InitAccessMatrix();*/
        }

        /*private void InitAccessMatrix()
        {
            try
            {
                LoadRights();
            }
            catch (FileNotFoundException)
            {
                core.Fs.SysWrite(datafile, "");
            }
        }

        private void LoadRights()
        {
            var data = core.Fs.SysRead(datafile);
            var lines = data.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int i;
            for (i = 0; lines[i] != string.Empty && i < lines.Length; i++)
            {
                var words = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                SubjectsLevels.Add(int.Parse(words[0]), int.Parse(words[1]));
            }
            for (i++; lines[i] != string.Empty && i < lines.Length; i++)
            {
                var words = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                ObjectsLevels.Add(int.Parse(words[0]), int.Parse(words[1]));
            }
        }

        private void SaveRights()
        {
            var sb = new StringBuilder();
            foreach (var p in SubjectsLevels)
                sb.AppendLine($"{p.Key} {p.Value}");
            sb.AppendLine();
            foreach (var p in ObjectsLevels)
                sb.AppendLine($"{p.Key} {p.Value}");
            core.Fs.SysWrite(datafile, sb.ToString());
        }*/
        public void Uninit()
        {
            //SaveRights();
        }



        //public SortedList<int, int> SubjectsLevels, ObjectsLevels;

        public int GetLevel(UserSubject sub) => sub.AccessLevel;
        public int GetLevel(FileObject obj) => obj.AccessLevel;

        public AccessRights Get(UserSubject sub, FileObject obj)
        {
            //if (!SubjectsLevels.ContainsKey(sub.Id) || !ObjectsLevels.ContainsKey(obj.Id))
            //    return AccessRights.None;
            if (sub.Type == UserType.Admin)
                return AccessRights.Full;

            if (GetLevel(sub) == GetLevel(obj))
                return AccessRights.Full;
            else if (GetLevel(sub) > GetLevel(obj))
                return AccessRights.Read;
            else
                return AccessRights.Write;
        }
        public bool Can(UserSubject sub, AccessRights rights, FileObject obj)
        {
            return Get(sub, obj).HasFlag(rights);
        }
        public void Check(UserSubject sub, AccessRights rights, FileObject obj)
        {
            if (!Can(sub, rights, obj))
                throw new HasNoRightsException(sub, ~Get(sub, obj) & rights, obj);
        }


        public void Set(UserSubject sub, int level) => sub.AccessLevel = level;
        public void Set(FileObject obj, int level) => obj.AccessLevel = level;

        //public void Remove(UserSubject sub) => SubjectsLevels.Remove(sub.Id);
        //public void Remove(FileObject obj) => ObjectsLevels.Remove(obj.Id);
    }

    class HasNoRightsException : OsException
    {
        public UserSubject Subject;
        public FileObject Object;
        public AccessRights NeededRights;
        public HasNoRightsException(UserSubject sub, AccessRights rights, FileObject obj) :
            base("Недостаточно следующих прав: " + rights.ToString())
        {
            Subject = sub;
            Object = obj;
            NeededRights = rights;
        }
    }
}
