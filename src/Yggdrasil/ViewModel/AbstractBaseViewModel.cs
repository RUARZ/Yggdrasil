using Yggdrasil.NotifyPropertyChange;

namespace Yggdrasil.ViewModel
{
    /// <summary>
    /// Abstract view model with base functionality.
    /// </summary>
    public abstract class AbstractBaseViewModel : NotifyPropertyChangedObject
    {
        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="AbstractBaseViewModel"/>
        /// </summary>
        protected AbstractBaseViewModel()
        {
            
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Bury the view model.
        /// </summary>
        protected void Bury()
        {
            ViewManager.BuryModel(this);
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Initializing method which get's called after creation and mapping between view and view model.
        /// </summary>
        public virtual void Initialize()
        {

        }

        #endregion
    }
}
