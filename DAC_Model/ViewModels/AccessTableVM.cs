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
        }

        public Commands.DelegateCommand BackCommand { get; private set; }

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

        public List<OS.UserSubject> Users
        {
            get { return OsService.GetOS().Umgr.Users; }
        }
        
        public ObservableCollection<OS.FileObject> Files
        {
            get { return OsService.GetOS().Fs.Files; }
        }
    }
}
