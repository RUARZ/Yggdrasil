using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Yggdrasil.Wpf.Linker
{
    class ItemsControlLinkHelper : IDisposable
    {
        #region Private Fields

        private readonly ItemsControl _control;
        private readonly Action<object, object, string> _createLinkAction;

        #endregion

        #region Constructor

        public ItemsControlLinkHelper(ItemsControl control, Action<object, object, string> createLinkAction)
        {
            _control = control;
            _createLinkAction = createLinkAction;
            _control.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        #endregion

        #region Interface Implementation

        public void Dispose()
        {
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

            foreach (FrameworkElement item in _control.Items.Cast<object>()
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
