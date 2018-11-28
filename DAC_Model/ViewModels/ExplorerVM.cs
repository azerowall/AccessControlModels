using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;

namespace DAC_Model.ViewModels
{
    class ExplorerVM : BaseVM
    {
        OS.Core os;
        
        public Commands.DelegateCommand OpenFileCommand { get; private set; }
        public Commands.DelegateCommand CreateFileCommand { get; private set; }
        public Commands.DelegateCommand RenameFileCommand { get; private set; }
        public Commands.DelegateCommand RemoveFileCommand { get; private set; }
        public Commands.DelegateCommand NavigateCommand { get; private set; }

        public ExplorerVM()
        {
            os = OsService.GetOS();
            OpenFileCommand = new Commands.DelegateCommand(OpenFile);
            CreateFileCommand = new Commands.DelegateCommand(CreateFile);
            RenameFileCommand = new Commands.DelegateCommand(RenameFile);
            RemoveFileCommand = new Commands.DelegateCommand(RemoveFile);
            NavigateCommand = new Commands.DelegateCommand(Navigate);
        }


        public OS.UserSubject CurrentUser
        {
            get { return os.CurrentUser; }
        }

        public string FileName { get; set; }

        /*IEnumerable<OS.FileObject> files;
        public IEnumerable<OS.FileObject> Files
        {
            get
            {
                files = os.Fs.Files.Where(f => os.RMon.Can(os.CurrentUser, OS.AccessRights.Read, f));
                return files;
            }
            set
            {
                files = value;
                OnPropertyChanged("Files");
            }
        }
        private void UpdateFilesList()
        {
            //Files = os.Fs.Files.Where(f => os.RMon.Can(os.CurrentUser, OS.AccessRights.Read, f));
            OnPropertyChanged("Files");
        }*/
        public ObservableCollection<OS.FileObject> Files
        {
            get { return os.Fs.Files; }
        }

        public OS.FileObject SelectedFile { get; set; }


        void CreateFile(object o)
        {
            try
            {
                string res = InputDialogService.Show("Создание файла", "Введите имя файла");
                if (res != null)
                    os.Fs.Create(res);
                //UpdateFilesList();
            }
            catch (OS.OsException e)
            {
                MessageBox.Show(e.Message);
            }
        }
        void OpenFile(object o)
        {
            try
            {
                if (SelectedFile == null) return;
                string fileData;
                if (os.RMon.Can(os.CurrentUser, OS.AccessRights.Read, SelectedFile))
                    fileData = os.Fs.Read(SelectedFile);
                else
                    fileData = string.Empty;
                
                string r = EditFileDialogService.Show(SelectedFile.Path, fileData);
                if (r != null)
                    os.Fs.Write(SelectedFile, r);
            }
            catch (OS.OsException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        void RenameFile(object o)
        {
            try
            {
                if (SelectedFile == null)
                    return;
                string newName = InputDialogService.Show("Переименование файла", "Введите новое имя");
                if (newName != null)
                    os.Fs.Rename(SelectedFile, newName);
                //UpdateFilesList();
            }
            catch (OS.OsException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        void RemoveFile(object o)
        {
            try
            {
                os.Fs.Remove(SelectedFile);
                //UpdateFilesList();
            }
            catch (OS.OsException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        void Navigate(object o)
        {
            switch ((string)o)
            {
                case "Access":
                    if (os.CurrentUser.Type != OS.UserType.Admin)
                        MessageBox.Show("Доступ есть только у администратора");
                    else
                        NavigationService.Navigate(NavigationService.AccessTablePage);
                    break;
                case "Logout":
                    os.Logout();
                    NavigationService.Navigate(NavigationService.LoginPage);
                    break;
                default:
                    MessageBox.Show((string)o);
                    break;
            }
        }

    }
}
