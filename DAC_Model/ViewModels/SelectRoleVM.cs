using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DAC_Model.ViewModels
{
    class SelectRoleVM : BaseVM
    {
        public SelectRoleVM()
        {
            SelectRoleCommand = new Commands.DelegateCommand(o => {
                var os = OsService.GetOS();
                os.SelectRole(SelectedUserRole);
                if (os.Sessions.HasSession(os.SessionId))
                {
                    var res = MessageBox.Show("Продолжить существующую сессию?", "Сессия", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        NavigationService.Navigate(NavigationService.ExplorerPage);
                        return;
                    }
                }
                os.Sessions.StartSession(os.SessionId);
                NavigationService.Navigate(NavigationService.ExplorerPage);
            });
        }

        public IEnumerable<OS.UserRole> UserRoles =>
            OsService.GetOS().GetUserRoles();
        public OS.UserRole SelectedUserRole { get; set; }

        public Commands.DelegateCommand SelectRoleCommand { get; private set; }

    }
}
