using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Yggdrasil.NotifyPropertyChange
{
    /// <summary>
    /// Handles the raising of <see cref="INotifyPropertyChanged.PropertyChanged"/> of derived classes.
    /// </summary>
    public abstract class NotifyPropertyChangedObject : INotifyPropertyChanged
    {
        #region Private Fields

        /// <summary>
        /// Internal list with property values of derived classes.
        /// </summary>
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();
        /// <summary>
        /// List of property names for which a notify on property change should be executed.
        /// </summary>
        private readonly List<string> _notifyOnPropertyChangePropertyNames = new List<string>();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="NotifyPropertyChangedObject"/> and searches for properties
        /// with the <see cref="NotifyOnPropertyChangeAttribute"/>!
        /// </summary>
        protected NotifyPropertyChangedObject()
        {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                if (property.GetCustomAttribute<NotifyOnPropertyChangeAttribute>() != null)
                    _notifyOnPropertyChangePropertyNames.Add(property.Name);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Set's the <paramref name="value"/> to the property with the name <paramref name="name"/> and checks if the value has changed.
        /// If the value has changed then <see langword="true"/> will be returned, otherwise <see langword="false"/>.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="value">The value of the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns><see langword="True"/> if the property has changed, otherwise <see langword="false"/>.</returns>
        protected bool SetValue<T>(T value, [CallerMemberName] string name = null)
        {
            bool notifyOnPropertyChange = _notifyOnPropertyChangePropertyNames.Contains(name);

            if (!_values.ContainsKey(name))
            {
                _values.Add(name, value);

                if (notifyOnPropertyChange)
                    FireNotifyPropertyChange(name);

                return true;
            }
            else
            {
                T oldValue = GetValue<T>(name);
                bool valueChanged = !Equals(oldValue, value);

                _values[name] = value;

                if (valueChanged && notifyOnPropertyChange)
                    FireNotifyPropertyChange(name);

                return valueChanged;
            }
        }

        /// <summary>
        /// Returns the actual value for <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property to return.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <returns>The actual value for <paramref name="name"/>.</returns>
        protected T GetValue<T>([CallerMemberName] string name = null)
        {
            if (!_values.ContainsKey(name))
                return default;

            return (T)_values[name];
        }

        /// <summary>
        /// Fires the <see cref="INotifyPropertyChanged.PropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property for the event.</param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            FireNotifyPropertyChange(propertyName);
        }

        #endregion

        #region Private Methods

        private void FireNotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Interface Implementations

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
