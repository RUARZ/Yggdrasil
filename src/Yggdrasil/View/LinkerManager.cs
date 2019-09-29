using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Yggdrasil
{
    /// <summary>
    /// Handles the searching and creating of implementations of <see cref="ILinker"/>.
    /// </summary>
    static class LinkerManager
    {
        #region Private Fields

        private static readonly Dictionary<Type, Type> _linker = new Dictionary<Type, Type>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Get's a <see cref="ILinker"/> implementation for the passed <paramref name="controlType"/>. If no implementation for the <paramref name="controlType"/> was
        /// registered then <see langword="null"/> will be returned.
        /// </summary>
        /// <param name="controlType">The <see cref="Type"/> of the control for getting a <see cref="ILinker"/> implementation.</param>
        /// <returns>The implementation for the <paramref name="controlType"/>, if no <see cref="ILinker"/> is registered then <see langword="null"/> will be
        /// returned.</returns>
        public static ILinker GetLinkerForType(Type controlType)
        {
            if (_linker.ContainsKey(controlType))
                return Activator.CreateInstance(_linker[controlType]) as ILinker;

            ILinker linker;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type type in assembly.GetTypes().Where(x => typeof(ILinker).IsAssignableFrom(x)))
                {
                    RegisterLinkerAttribute att = type.GetCustomAttribute<RegisterLinkerAttribute>();
                    if (att == null)
                        continue;

                    if (att.ControlType == controlType)
                    {
                        linker = Activator.CreateInstance(type) as ILinker;
                        _linker.Add(controlType, type);
                        return linker;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
