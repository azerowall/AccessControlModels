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

        public Commands.DelegateCommand SaveFileCommand { get; private set; }
        public Commands.DelegateCommand OpenFileCommand { get; private set; }
        public Commands.DelegateCommand CreateFileCommand { get; private set; }
        public Commands.DelegateCommand CloseFileCommand { get; private set; }
        public Commands.DelegateCommand RenameFileCommand { get; private set; }
        public Commands.DelegateCommand RemoveFileCommand { get; private set; }
        public Commands.DelegateCommand NavigateCommand { get; private set; }

        public ExplorerVM()
        {
            os = OsService.GetOS();
            SaveFileCommand = new Commands.DelegateCommand(SaveFile, o => CurrentFile != null);
            OpenFileCommand = new Commands.DelegateCommand(OpenFile);
            CreateFileCommand = new Commands.DelegateCommand(CreateFile);
            CloseFileCommand = new Commands.DelegateCommand(CloseFile);
            RenameFileCommand = new Commands.DelegateCommand(RenameFile);
            RemoveFileCommand = new Commands.DelegateCommand(RemoveFile);
            NavigateCommand = new Commands.DelegateCommand(Navigate);
        }


        public OS.UserSubject CurrentUser
        {
            get { return os.CurrentUser; }
        }

        public string FileName { get; set; }

        IEnumerable<OS.FileObject> files;
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
        }

        public OS.FileObject SelectedFile { get; set; }

        OS.FileObject currentFile;
        public OS.FileObject CurrentFile
        {
            get { return currentFile; }
            set
            {
                currentFile = value;
                OnPropertyChanged("CurrentFile");
                SaveFileCommand.RaiseCanExecuteChanged();
            }
        }

        bool hasOpenedFile;
        public bool HasOpenedFile
        {
            get { return hasOpenedFile; }
            set { hasOpenedFile = value; OnPropertyChanged("HasOpenedFile"); }
        }

        string fileData;
        public string FileData
        {
            get { return fileData; }
            set
            {
                fileData = value;
                OnPropertyChanged("FileData");
            }
        }



        void CreateFile(object o)
        {
            try
            {
                string res = InputDialogService.Show("Создание файла", "Введите имя файла");
                if (res != null)
                    os.Fs.Create(res);
                UpdateFilesList();
            }
            catch (OS.FileSystemException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (OS.HasNoRightsException)
            {
                MessageBox.Show("Недостаточно прав");
            }
        }
        void OpenFile(object o)
        {
            try
            {
                var file = SelectedFile;
                FileData = os.Fs.Read(file);
                CurrentFile = file;
                HasOpenedFile = true;
            }
            catch (OS.FileSystemException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (OS.HasNoRightsException)
            {
                MessageBox.Show("Недостаточно прав");
            }
        }
        void CloseFile(object o)
        {
            HasOpenedFile = false;
            FileData = string.Empty;
            CurrentFile = null;
        }
        void SaveFile(object o)
        {
            try
            {
                os.Fs.Write(CurrentFile, FileData);
            }
            catch(OS.FileSystemException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (OS.HasNoRightsException)
            {
                MessageBox.Show("Недостаточно прав");
            }
        }

        void RenameFile(object o)
        {
            try
            {
                string newName = InputDialogService.Show("Переименование файла", "Введите новое имя");
                if (newName != null)
                    os.Fs.Rename(SelectedFile, newName);
                UpdateFilesList();
            }
            catch (OS.FileSystemException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (OS.HasNoRightsException)
            {
                MessageBox.Show("Недостаточно прав");
            }
        }

        void RemoveFile(object o)
        {
            try
            {
                os.Fs.Remove(SelectedFile);
                UpdateFilesList();
            }
            catch (OS.FileSystemException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (OS.HasNoRightsException)
            {
                MessageBox.Show("Недостаточно прав");
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
