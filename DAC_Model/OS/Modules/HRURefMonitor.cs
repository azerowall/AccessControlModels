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
        //Execute = 0x08,
        Remove = 0x04,
        Full = Read | Write | Remove
    }

    class HRURefMonitor
    {
        static readonly string datafile = "accessmatrix.os";
        Core core;

        public HRURefMonitor(Core core)
        {
            this.core = core;
            AccessMatrix = new List<List<AccessRights>>();
            InitAccessMatrix();
        }

        private void InitAccessMatrix()
        {
            try
            {
                LoadMatrix();
            }
            catch (FileNotFoundException)
            {
                core.Fs.SysWrite(datafile, "");
            }
        }

        private void LoadMatrix()
        {
            var data = core.Fs.SysRead(datafile);
            foreach (var line in data.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var words = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                AccessMatrix.Add(new List<AccessRights>());
                foreach (var w in words)
                    AccessMatrix[AccessMatrix.Count - 1].Add((AccessRights)int.Parse(w));
            }
        }

        private void SaveMatrix()
        {
            var sb = new StringBuilder();
            foreach (var row in AccessMatrix)
                sb.AppendLine(string.Join(" ", row.Select(rights => (int)rights)));
            core.Fs.SysWrite(datafile, sb.ToString());
        }
        public void Uninit()
        {
            SaveMatrix();
        }



        public List<List<AccessRights>> AccessMatrix;

        public AccessRights Get(IOsSubject sub, IOsObject obj)
        {
            if (sub.Id < AccessMatrix.Count && obj.Id < AccessMatrix[0].Count)
                return AccessMatrix[sub.Id][obj.Id];
            return AccessRights.None;
        }
        public bool Can(IOsSubject sub, AccessRights rights, IOsObject obj)
        {
            return Get(sub, obj).HasFlag(rights);
        }
        public void Check(IOsSubject sub, AccessRights rights, IOsObject obj)
        {
            if (!Can(sub, rights, obj))
                throw new HasNoRightsException(sub, rights, obj);
        }


        public void Set(IOsSubject sub, AccessRights rights, IOsObject obj)
        {
            
            while (sub.Id >= AccessMatrix.Count)
                AccessMatrix.Add(new List<AccessRights>());

            for (int irow = 0; irow < AccessMatrix.Count; irow++)
                while (obj.Id >= AccessMatrix[irow].Count)
                    AccessMatrix[irow].Add(AccessRights.None);

            /*if (obj.Id >= AccessMatrix[0].Count)
            {
                int delta = obj.Id - (AccessMatrix[0].Count - 1);
                for (int irow = 0; irow < AccessMatrix.Count; irow++)
                    AccessMatrix[irow].AddRange(new AccessRights[delta]);
            }*/

            AccessMatrix[sub.Id][obj.Id] = rights;
        }

        public void Remove(IOsSubject sub, IOsObject obj)
        {

        }
    }

    class HasNoRightsException : Exception
    {
        public IOsSubject Subject;
        public IOsObject Object;
        public AccessRights Rights;
        public HasNoRightsException(IOsSubject sub, AccessRights rights, IOsObject obj)
        {
            Subject = sub;
            Object = obj;
            Rights = rights;
        }
    }
}
