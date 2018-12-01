using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DAC_Model.ViewModels
{
    class LoginVM : BaseVM
    {
        string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }
        string password;
        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged("Password"); }
        }

        public Commands.DelegateCommand LoginCommand { get; set; }
        public Commands.DelegateCommand RegistrateCommand { get; set; }

        public LoginVM()
        {
            LoginCommand = new Commands.DelegateCommand(login);
            RegistrateCommand = new Commands.DelegateCommand(registrate);
            //Name = "гость"; Password = "guest";
        }

        public void login(object o)
        {
            try
            {
                OsService.GetOS().Login(Name, Password);
                Name = Password = string.Empty;
                NavigationService.Navigate(NavigationService.SelectRolePage);
            }
            catch (OS.UserMgrException e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public void registrate(object o)
        {
            try
            {
                OsService.GetOS().Umgr.Registrate(Name, Password);
                OsService.GetOS().Login(Name, Password);
                Name = Password = string.Empty;
                //NavigationService.Navigate(NavigationService.ExplorerPage);
            }
            catch (OS.UserMgrException e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
