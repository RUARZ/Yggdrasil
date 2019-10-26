using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using Yggdrasil.Wpf.Helper;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(GridViewColumn))]
    public class GridViewColumnLinker : ILinker
    {
        #region Interface Implementation

        public void Link(object viewElement, IEnumerable<LinkData> linkData, Action<object, object, string> createLinkAction)
        {
            if (!(viewElement is GridViewColumn gridViewColumn))
                return;

            foreach (LinkData data in linkData)
            {
                PropertyInfo pInfo = data.ContextMemberInfo as PropertyInfo;

                switch (data.ViewElementName)
                {
                    case nameof(GridViewColumn.DisplayMemberBinding):
                        gridViewColumn.DisplayMemberBinding = BindingHandler.CreateBinding(pInfo);
                        break;
                    default:
                        throw new NotSupportedException($"Property '{data.ViewElementName}' is not supported by {GetType().Name}!");
                }
            }
        }

        public void Unlink()
        {
            //nothing to do
        }

        #endregion
    }
}
