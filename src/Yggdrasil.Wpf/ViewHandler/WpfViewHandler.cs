using System;
using System.Windows;

namespace Yggdrasil.Wpf.ViewHandler
{
    public class WpfViewHandler : IViewHandler
    {
        #region Private Fields

        private Func<object, object> _getViewForViewModelFunc;

        #endregion

        #region Interface Implementation

        public void Bury(object view)
        {
            Window window = (Window)view;
            DerigsterClosedEvent(window);

            window.Close();
        }

        public void Display(object view)
        {
            Window window = (Window)view;
            RegisterClosedEvent(window);

            window.Show();
        }

        public void DisplayModal(object owner, object view)
        {
            Window ownerWindow = _getViewForViewModelFunc.Invoke(owner) as Window;
            Window modalWindow = (Window)view;
            RegisterClosedEvent(modalWindow);
            modalWindow.Owner = ownerWindow;
            modalWindow.ShowDialog();
        }

        public object GetContext(object view)
        {
            return ((Window)view).DataContext;
        }

        public void RegisterGetViewForViewModelFunction(Func<object, object> getViewForViewModelFunc)
        {
            _getViewForViewModelFunc = getViewForViewModelFunc;
        }

        public void SetContext(object view, object viewModel)
        {
            ((Window)view).DataContext = viewModel;
        }

        #endregion

        #region Private Methods

        private void RegisterClosedEvent(Window window)
        {
            window.Closed += Window_Closed;
        }

        private void DerigsterClosedEvent(Window window)
        {
            window.Closed -= Window_Closed;
        }

        #endregion

        #region Event Handling

        private void Window_Closed(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            DerigsterClosedEvent(window);

            ViewManager.BuryModel(window.DataContext);
        }

        #endregion
    }
}
