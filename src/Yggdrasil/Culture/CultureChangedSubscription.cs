using System;
using System.Globalization;

namespace Yggdrasil.Culture
{
    /// <summary>
    /// Represents a subscription to a culture change of <see cref="CultureManager"/>.
    /// </summary>
    public class CultureChangedSubscription : IDisposable
    {
        #region Private Fields

        private readonly Action<CultureChangedSubscription> _disposeAction;
        private readonly Action<CultureInfo> _cultureChangedAction;

        #endregion

        #region Constructor
        
        /// <summary>
        /// Creates a new <see cref="CultureChangedSubscription"/> and sets its <see cref="Action"/>s.
        /// </summary>
        /// <param name="cultureChangedAction"><see cref="Action"/> which should be executed on a culture change.</param>
        /// <param name="disposeAction"><see cref="Action"/> which should be executed to dispose the subscription.</param>
        internal CultureChangedSubscription(Action<CultureInfo> cultureChangedAction, Action<CultureChangedSubscription> disposeAction)
        {
            _cultureChangedAction = cultureChangedAction;
            _disposeAction = disposeAction;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Invokes the culture changed subscription <see cref="Action"/>.
        /// </summary>
        /// <param name="culture">The new active <see cref="CultureInfo"/>.</param>
        public void ExecuteCultureChanged(CultureInfo culture)
        {
            _cultureChangedAction?.Invoke(culture);
        }

        #endregion

        #region Interface Implementation

        public void Dispose()
        {
            _disposeAction?.Invoke(this);
        }

        #endregion
    }
}
