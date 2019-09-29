using System;

namespace Yggdrasil.Transposer
{
    /// <summary>
    /// Custom attribute for registering a transposer to a specific view type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RegisterTransposerAttribute : Attribute
    {
        /// <summary>
        /// Creates an instance of <see cref="RegisterTransposerAttribute"/> and sets <see cref="ViewType"/> to <paramref name="viewType"/>.
        /// </summary>
        /// <param name="viewType">The type of the view for which the transposer should be used.</param>
        public RegisterTransposerAttribute(Type viewType)
        {
            ViewType = viewType;
        }

        /// <summary>
        /// <see cref="Type"/> of the view for the transposer.
        /// </summary>
        public Type ViewType { get; }
    }
}
