using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace DAC_Model.ViewModels
{
    class AccessTableVM : BaseVM
    {
        public AccessTableVM()
        {
            //SetRightCommand = new Commands.DelegateCommand(SetRight);
            BackCommand = new Commands.DelegateCommand(o =>
                                NavigationService.Navigate(NavigationService.ExplorerPage));
            AddRoleCommand = new Commands.DelegateCommand(o =>
                                    {
                                        OsService.GetOS().RMon.AddRole(RoleName);
                                        RoleName = string.Empty;
                                        OnPropertyChanged("Roles");
                                    });
            RemoveRoleCommand = new Commands.DelegateCommand(o =>
            {
                OsService.GetOS().RMon.RemoveRole(SelectedRole);
                OnPropertyChanged("Roles");
                OnPropertyChanged("UsersRoles");
            });

            AddRoleToUserCommand = new Commands.DelegateCommand(o =>
            {
                OsService.GetOS().RMon.AddRoleToUser(SelectedUser, SelectedUserRole);
                OnPropertyChanged("UsersRoles");
            });
            RemoveRoleFromUserCommand = new Commands.DelegateCommand(o =>
            {
                OsService.GetOS().RMon.RemoveRoleFromUser(SelectedUser, SelectedUserRole);
                OnPropertyChanged("UsersRoles");
            });
        }

        public Commands.DelegateCommand BackCommand { get; private set; }
        public Commands.DelegateCommand AddRoleCommand { get; private set; }
        public Commands.DelegateCommand RemoveRoleCommand { get; private set; }
        public Commands.DelegateCommand AddRoleToUserCommand { get; private set; }
        public Commands.DelegateCommand RemoveRoleFromUserCommand { get; private set; }


        string roleName;
        public string RoleName
        {
            get { return roleName; }
            set { roleName = value; OnPropertyChanged("RoleName"); }
        }
        public OS.UserRole SelectedRole { get; set; }


        public OS.UserSubject SelectedUser { get; set; }
        public OS.UserRole SelectedUserRole { get; set; }


        public IEnumerable<object[]> UsersRoles
        {
            get
            {
                var os = OsService.GetOS();
                return os.Umgr.Users
                    .Select(u => new object[] {
                                u,
                                string.Join(", ", os.RMon.GetUserRoles(u.Id))
                            });
            }
        }

        /*
        public IEnumerable<string> AccessRights
        {
            get
            {
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
        }*/

        public IEnumerable<OS.UserRole> Roles =>
            OsService.GetOS().RMon.Roles.Values.ToArray();

        public List<OS.UserSubject> Users =>
            OsService.GetOS().Umgr.Users;

        public ObservableCollection<OS.FileObject> Files =>
            OsService.GetOS().Fs.Files;
    }
}
