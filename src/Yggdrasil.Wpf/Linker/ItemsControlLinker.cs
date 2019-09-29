using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(ItemsControl))]
    public class ItemsControlLinker : ILinker
    {
        #region Private Fields

        private ItemsControl _control;
        private Action<object, object, string> _createLinkAction;

        #endregion

        #region Interface Implementation

        public void Link(object control, object context, Dictionary<string, MemberInfo> linkDefinitions, Action<object, object, string> createLinkAction)
        {
            if (!(control is ItemsControl itemsControl))
                return;

            _control = itemsControl;
            _createLinkAction = createLinkAction;

            foreach (KeyValuePair<string, MemberInfo> definition in linkDefinitions)
            {
                switch (definition.Key)
                {
                    case nameof(ItemsControl.ItemsSource):
                        if (!(definition.Value is PropertyInfo propInfo))
                            throw new NotSupportedException($"The {nameof(ItemsControl.ItemsSource)} must be linked to a property!");

                        if (!(typeof(IEnumerable).IsAssignableFrom(propInfo.PropertyType)))
                            throw new NotSupportedException($"The {nameof(ItemsControl.ItemsSource)} must be linked to a implementation of '{typeof(IEnumerable)}'!");

                        _control.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;

                        Binding binding = new Binding(propInfo.Name);
                        binding.Source = context;
                        binding.Mode = BindingMode.OneWay;
                        BindingOperations.SetBinding(itemsControl, ItemsControl.ItemsSourceProperty, binding);
                        break;
                    default:
                        throw new NotSupportedException($"The property name '{definition.Key}' is not supported by '{GetType().Name}'!");
                }
            }
        }

        public void Unlink()
        {
            if (_control == null)
                return;

            _control.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
        }

        #endregion

        #region Private Methods

        private void CreateLink(DependencyObject obj, object context)
        {
            if (obj == null)
                return;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                _createLinkAction(child, context, child.GetValue(FrameworkElement.NameProperty) as string);

                if (VisualTreeHelper.GetChildrenCount(child) > 0)
                    CreateLink(child, context);
            }
        }

        #endregion

        #region Events

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (_control.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                return;

            foreach (FrameworkElement item in  _control.Items.Cast<object>()
                .Select(x => (FrameworkElement)_control.ItemContainerGenerator.ContainerFromItem(x)).Where(x => !x.IsLoaded))
            {
                item.Loaded += Item_Loaded;
            }
        }

        private void Item_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            element.Loaded -= Item_Loaded;

            CreateLink(element, _control.ItemContainerGenerator.ItemFromContainer(element));
        }

        #endregion
    }
}
