﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DAC_Model.Pages
{
    /// <summary>
    /// Логика взаимодействия для AccessTable.xaml
    /// </summary>
    public partial class AccessTable : Page
    {
        public AccessTable()
        {
            InitializeComponent();
            DataContext = new ViewModels.AccessTableVM();
        }
    }
}
