﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DAC_Model
{
    static class NavigationService
    {
        static ViewModels.Navigator nav;
        public static ViewModels.Navigator Navigator
        {
            get
            {
                if (nav == null)
                {
                    nav = new ViewModels.Navigator();
                    nav.Navigate(LoginPage);
                }
                return nav;
            }
        }
        public static ViewModels.Navigator GetNavigator() => Navigator;
        public static void Navigate(Page page) => Navigator.Navigate(page);

        static Page loginPage;
        public static Page LoginPage =>
            loginPage == null ? loginPage = new Pages.Login() : loginPage;

        static Page explorerPage;
        public static Page ExplorerPage =>
            explorerPage == null ? explorerPage = new Pages.Explorer() : explorerPage;

        static Page accessTablePage;
        public static Page AccessTablePage =>
            accessTablePage == null ? accessTablePage = new Pages.AccessTable() : accessTablePage;

        static Page selectRolePage;
        public static Page SelectRolePage =>
            selectRolePage == null ? selectRolePage = new Pages.SelectRole() : selectRolePage;

    }
}
