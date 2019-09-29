using System.ComponentModel;
using Yggdrasil.ViewModel;

namespace Yggdrasil.Wpf.ViewModel
{
    /// <summary>
    /// Base view model.
    /// </summary>
    public abstract class ViewModel : AbstractBaseViewModel, INotifyPropertyChanged
    {
        #region Method Overrides

        protected override void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Interface Implementation

        #region INotifyPropertyChange

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        
        #endregion
    }
}
