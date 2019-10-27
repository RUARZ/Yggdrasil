using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace Yggdrasil.Wpf.Helper
{
    /// <summary>
    /// Provides methods for creating and return a <see cref="Binding"/> or set the <see cref="Binding"/> to a passed control.
    /// </summary>
    static class BindingHandler
    {
        #region Public Methods

        /// <summary>
        /// Set's a <see cref="Binding"/> to the passed <paramref name="target"/> on <paramref name="property"/>.
        /// </summary>
        /// <param name="target">The target element on which the binding should be set.</param>
        /// <param name="property">The <see cref="DependencyProperty"/> on which the binding should be created.</param>
        /// <param name="pInfo"><see cref="PropertyInfo"/> of the source to which the binding should be set.</param>
        /// <param name="source">The source of the <see cref="Binding"/> if <see langword="null"/> then the source will not be set explicity.</param>
        /// <returns>The created <see cref="BindingExpressionBase"/> from the <see cref="BindingOperations.SetBinding(DependencyObject, DependencyProperty, BindingBase)"/>.</returns>
        public static BindingExpressionBase SetBinding(DependencyObject target, DependencyProperty property, PropertyInfo pInfo, object source = null, IEnumerable<PropertyInfo> propertyPath = null)
        {
            Binding binding = CreateBinding(pInfo, source, propertyPath);

            return BindingOperations.SetBinding(target, property, binding);
        }

        /// <summary>
        /// Creates a <see cref="Binding"/> instance and set's the values depending on passed <paramref name="pInfo"/>.
        /// </summary>
        /// <param name="pInfo"><see cref="PropertyInfo"/> of the property to which the binding should be created.</param>
        /// <param name="source">The source for the <see cref="Binding"/>, if <see langword="null"/> then the <see cref="Binding.Source"/> will not be set.</param>
        /// <returns>The created <see cref="Binding"/>.</returns>
        public static Binding CreateBinding(PropertyInfo pInfo, object source = null, IEnumerable<PropertyInfo> propertyPath = null)
        {
            string path;

            if (propertyPath == null)
                path = pInfo.Name;
            else
                path = CreatePropertyPath(propertyPath, pInfo);

            Binding binding = new Binding(path);

            if (source != null)
                binding.Source = source;

            binding.Mode = GetBindingMode(pInfo);

            return binding;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the <see cref="BindingMode"/> for a <see cref="Binding"/> depending on the accessibility of the passed <paramref name="pInfo"/>.
        /// </summary>
        /// <param name="pInfo"><see cref="PropertyInfo"/> to which a <see cref="Binding"/> should be created.</param>
        /// <returns>The <see cref="BindingMode"/> corresponding to the passed <paramref name="pInfo"/>.</returns>
        private static BindingMode GetBindingMode(PropertyInfo pInfo)
        {
            bool isGetterPublic = pInfo.GetGetMethod(true)?.IsPublic ?? false;
            bool isSetterPublic = pInfo.GetSetMethod(true)?.IsPublic ?? false;

            if (isSetterPublic && isGetterPublic)
                return BindingMode.TwoWay;

            if (isSetterPublic && !isGetterPublic)
                return BindingMode.OneWayToSource;

            if (!isSetterPublic && isGetterPublic)
                return BindingMode.OneWay;

            throw new NotSupportedException($"The getter and the setter of the property '{pInfo.Name}' are not accessible!");
        }

        private static string CreatePropertyPath(IEnumerable<PropertyInfo> propertyPath, PropertyInfo targetProperty)
        {
            return string.Join(".", propertyPath.Select(x => x.Name)) + "." + targetProperty.Name;
        }

        #endregion
    }
}
