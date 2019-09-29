using System;

namespace Yggdrasil
{
    /// <summary>
    /// <see cref="Attribute"/> for register a implementation of <see cref="ILinker"/> for a specific control type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RegisterLinkerAttribute : Attribute
    {
        /// <summary>
        /// Creates a instance of <see cref="RegisterLinkerAttribute"/> and sets <see cref="ControlType"/> to <paramref name="controlType"/>.
        /// </summary>
        /// <param name="controlType">The <see cref="Type"/> of the control for registering the <see cref="ILinker"/> implementation.</param>
        public RegisterLinkerAttribute(Type controlType)
        {
            ControlType = controlType;
        }

        /// <summary>
        /// <see cref="Type"/> of the control to register a <see cref="ILinker"/> implementation.
        /// </summary>
        public Type ControlType { get; }
    }
}
