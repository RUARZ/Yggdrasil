using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;

namespace Yggdrasil.Wpf.Helper
{
    /// <summary>
    /// Handles the <see cref="INotifyPropertyChanged.PropertyChanged"/> registration and event calls of added models and calls
    /// passed actions.
    /// </summary>
    class PropertyChangedHandler : IDisposable
    {
        #region Private Methods

        private readonly Dictionary<INotifyPropertyChanged, Dictionary<string, Action>> _registerdNotifyPropertyChangedActions = new Dictionary<INotifyPropertyChanged, Dictionary<string, Action>>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the passed <paramref name="item"/> to the items which should be handeled by this <see cref="PropertyChangedHandler"/>.
        /// If the <paramref name="item"/> raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event with the passed <paramref name="propertyName"/>
        /// then the passed <paramref name="action"/> will be executed.
        /// </summary>
        /// <param name="item">The item to register the event.</param>
        /// <param name="propertyName">The name of the property to handle.</param>
        /// <param name="action">The <see cref="Action"/> which should be executed on the change.</param>
        public void AddNotifyPropertyChangedItem(INotifyPropertyChanged item, string propertyName, Action action)
        {
            if (!_registerdNotifyPropertyChangedActions.ContainsKey(item))
            {
                _registerdNotifyPropertyChangedActions.Add(item, new Dictionary<string, Action>());
                item.PropertyChanged += NotifyPropertyChangedItem_PropertyChanged;
            }

            try
            {
                _registerdNotifyPropertyChangedActions[item].Add(propertyName, action);
            }
            catch (DuplicateKeyException ex)
            {
                throw new Exception($"The key '{propertyName}' was alread added!", ex);
            }
        }

        #endregion

        #region Event Handling

        private void NotifyPropertyChangedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            INotifyPropertyChanged item = sender as INotifyPropertyChanged;

            if (item == null)
                return;

            if (!_registerdNotifyPropertyChangedActions.ContainsKey(item))
                return;

            if (!_registerdNotifyPropertyChangedActions[item].ContainsKey(e.PropertyName))
                return;

            _registerdNotifyPropertyChangedActions[item][e.PropertyName].Invoke();
        }

        #endregion

        #region Interface Implementation

        public void Dispose()
        {
            foreach (var item in _registerdNotifyPropertyChangedActions)
            {
                item.Key.PropertyChanged -= NotifyPropertyChangedItem_PropertyChanged; 
            }
        }

        #endregion
    }
}
