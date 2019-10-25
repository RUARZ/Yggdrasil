using System;

namespace Yggdrasil.NotifyPropertyChange
{
    /// <summary>
    /// Defines if the notify for property changes should be called after property change. Only compatible with <see cref="AbstractBaseViewModel.SetValue{T}(string, object)"/>
    /// method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NotifyOnPropertyChangeAttribute : Attribute
    {
    }
}
