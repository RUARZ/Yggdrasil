using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Yggdrasil.ViewModel
{
    /// <summary>
    /// Abstract view model with base functionality.
    /// </summary>
    public abstract class AbstractBaseViewModel
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
        /// Creates a new instance and 
        /// </summary>
        protected AbstractBaseViewModel()
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
                    NotifyPropertyChange(name);

                return true;
            }
            else
            {
                T oldValue = GetValue<T>(name);
                bool valueChanged = !Equals(oldValue, value);

                _values[name] = value;

                if (valueChanged && notifyOnPropertyChange)
                    NotifyPropertyChange(name);

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
        /// Bury the view model.
        /// </summary>
        protected void Bury()
        {
            ViewManager.BuryModel(this);
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Executes the notify property change.
        /// </summary>
        /// <param name="propertyName">The name of the property to notify property change.</param>
        protected abstract void NotifyPropertyChange(string propertyName);

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
