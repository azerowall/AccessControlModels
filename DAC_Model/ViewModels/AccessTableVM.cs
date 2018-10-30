using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DAC_Model.ViewModels
{
    class AccessTableVM : BaseVM
    {
        public AccessTableVM()
        {
            //ColumnsCount = OsService.GetOS().Fs.Files.Count + 1;
            //UpdateAccessTable();
            SetRightCommand = new Commands.DelegateCommand(SetRight);
            BackCommand = new Commands.DelegateCommand(o =>
                                NavigationService.Navigate(NavigationService.ExplorerPage));
        }

        public Commands.DelegateCommand BackCommand { get; private set; }

        public int ColumnsCount { get; private set; }

        //IEnumerable<string> accessRights;
        public IEnumerable<string> AccessRights
        {
            get
            {
                ColumnsCount = OsService.GetOS().Fs.Files.Count + 1;
                return VisualizeTable().SelectMany(x => x);
            }
        }


        private List<List<string>> VisualizeTable()
        {
            var table = new List<List<string>>();

            var os = OsService.GetOS();

            // верхний заголовок (имена файлов)
            table.Add(new List<string>());
            table[0].Add(string.Empty);
            foreach (var file in os.Fs.Files)
                table[0].Add(file.Path);

            foreach (var user in os.Umgr.Users)
            {
                table.Add(new List<string>());
                table[table.Count - 1].Add(user.Name);

                foreach (var file in os.Fs.Files)
                {
                    table[table.Count - 1].Add(os.RMon.Get(user, file).ToString());
                }
            }
            return table;
        }


        public OS.UserSubject SelectedUser { get; set; }
        public IEnumerable<OS.UserSubject> Users
        {
            get { return OsService.GetOS().Umgr.Users; }
        }

        public OS.FileObject SelectedFile { get; set; }
        public IEnumerable<OS.FileObject> Files
        {
            get { return OsService.GetOS().Fs.Files; }
        }

        bool isReadAccess;
        public bool IsReadAccess
        {
            get { return isReadAccess; }
            set { isReadAccess = value; OnPropertyChanged("IsReadAccess"); }
        }
        bool isWriteAccess;
        public bool IsWriteAccess
        {
            get { return isWriteAccess; }
            set { isWriteAccess = value; OnPropertyChanged("IsWriteAccess"); }
        }
        bool isRemoveAccess;
        public bool IsRemoveAccess
        {
            get { return isRemoveAccess; }
            set { isRemoveAccess = value; OnPropertyChanged("IsRemoveAccess"); }
        }

        public Commands.DelegateCommand SetRightCommand { get; private set; }

        private void SetRight(object o)
        {
            var os = OsService.GetOS();

            if (os.CurrentUser.Type != OS.UserType.Admin)
            {
                MessageBox.Show("Только администратор может устанавливать права доступа");
                return;
            }
            if (SelectedUser == null || SelectedFile == null)
                return;

            OS.AccessRights rights = OS.AccessRights.None;
            if (IsReadAccess)
                rights |= OS.AccessRights.Read;
            if (IsWriteAccess)
                rights |= OS.AccessRights.Write;
            if (IsRemoveAccess)
                rights |= OS.AccessRights.Remove;
            os.RMon.Set(SelectedUser, rights, SelectedFile);

            OnPropertyChanged("AccessRights");
        }
    }
}
