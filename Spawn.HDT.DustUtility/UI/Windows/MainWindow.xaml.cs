﻿#region Using
using Microsoft.Practices.ServiceLocation;
using Spawn.HDT.DustUtility.UI.ViewModels;
#endregion

namespace Spawn.HDT.DustUtility.UI.Windows
{
    public partial class MainWindow
    {
        #region Ctor
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        #region OnClosing
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) => ServiceLocator.Current.GetInstance<MainViewModel>().OnClosing();
        #endregion
        #endregion
    }
}