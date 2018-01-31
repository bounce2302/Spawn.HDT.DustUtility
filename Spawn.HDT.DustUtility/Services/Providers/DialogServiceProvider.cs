﻿#region Using
using Spawn.HDT.DustUtility.UI.ViewModels;
using System.Windows;
#endregion

namespace Spawn.HDT.DustUtility.Services.Providers
{
    public class DialogServiceProvider : IDialogService
    {
        #region Member Variables
        private Window m_dialog;
        #endregion

        #region ShowDialog
        public bool ShowDialog<T>(Window owner = null) where T : Window, new()
        {
            m_dialog = new T();

            if (owner != null)
            {
                m_dialog.Owner = owner;
            }
            else { }

            (m_dialog.DataContext as ViewModelBase).Initialize();

            return (m_dialog.ShowDialog() ?? false);
        }
        #endregion

        #region GetDialogResult
        public T GetDialogResult<T>()
        {
            T retVal = default(T);

            if (m_dialog?.DataContext is IResultProvider<T> resultProvider)
            {
                retVal = resultProvider.GetResult();
            }
            else { }

            return retVal;
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            m_dialog = null;
        }
        #endregion
    }
}