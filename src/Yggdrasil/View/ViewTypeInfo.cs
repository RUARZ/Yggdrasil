using System;
using System.Reflection;

namespace Yggdrasil
{
    /// <summary>
    /// Provides infos for a view type.
    /// </summary>
    class ViewTypeInfo
    {
        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="ViewTypeInfo"/> and sets the needed infos for the view.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the view.</param>
        /// <param name="contextPropertyName">Name of the context property.</param>
        /// <param name="displayMethodName">Name of the display method for the view.</param>
        /// <param name="buryMethodName">Name of the bury method of the view.</param>
        /// <param name="buriedEventName">Name of the event after a view was buried.</param>
        public ViewTypeInfo(Type type, string contextPropertyName, string displayMethodName, string buryMethodName, string buriedEventName)
        {
            Type = type;
            ContextPropertyInfo = type.GetProperty(contextPropertyName);
            DisplayMethodInfo = type.GetMethod(displayMethodName);
            BuryMethodInfo = type.GetMethod(buryMethodName);
            BuriedEventInfo = type.GetEvent(buriedEventName);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The <see cref="Type"/> of the view for this info.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// <see cref="PropertyInfo"/> for the context property of a view.
        /// </summary>
        public PropertyInfo ContextPropertyInfo { get; }

        /// <summary>
        /// <see cref="MethodInfo"/> for the display method of a view.
        /// </summary>
        public MethodInfo DisplayMethodInfo { get; }

        /// <summary>
        /// <see cref="MethodInfo"/> to bury a view.
        /// </summary>
        public MethodInfo BuryMethodInfo { get; }

        /// <summary>
        /// <see cref="EventInfo"/> of the event to execute actions after a view is buried.
        /// </summary>
        public EventInfo BuriedEventInfo { get; }

        #endregion
    }
}
