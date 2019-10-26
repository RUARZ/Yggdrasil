using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using Yggdrasil.Wpf.Helper;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(DataGridBoundColumn))]
    public class DataGridBoundColumnLinker : ILinker
    {
        public void Link(object viewElement, IEnumerable<LinkData> linkData, Action<object, object, string> createLinkAction)
        {
            if (!(viewElement is DataGridBoundColumn column))
                return;

            foreach (LinkData data in linkData)
            {
                PropertyInfo pInfo = data.ContextMemberInfo as PropertyInfo;

                switch (data.ViewElementName)
                {
                    case nameof(DataGridBoundColumn.Binding):
                        column.Binding = BindingHandler.CreateBinding(pInfo);
                        break;
                    default:
                        throw new NotSupportedException($"The property name '{data.ViewElementName}' is not supported by '{GetType().Name}'!");
                }
            }
        }

        public void Unlink()
        {
            
        }
    }
}
