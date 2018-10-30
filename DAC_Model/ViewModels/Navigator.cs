using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DAC_Model.ViewModels
{
    class Navigator : ViewModels.BaseVM
    {
        Page currentPage;
        public Page CurrentPage
        {
            get { return currentPage; }
            private set
            {
                currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }

        public void Navigate(Page page)
        {
            // этот трюк обновит биндинги
            var t = page.DataContext;
            page.DataContext = null;
            page.DataContext = t;

            CurrentPage = page;
        }
    }
}
