using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(DataGridBoundColumn))]
    public class DataGridBoundColumnLinker : ILinker
    {
        public void Link(object control, object context, Dictionary<string, string> linkDefinitions, Dictionary<string, MemberInfo> foundLinks, Action<object, object, string> createLinkAction)
        {
            if (!(control is DataGridBoundColumn column))
                return;

            foreach (KeyValuePair<string, string> definition in linkDefinitions)
            {
                switch(definition.Key)
                {
                    case nameof(DataGridBoundColumn.Binding):
                        column.Binding = new Binding(definition.Value);
                        break;
                    default:
                        throw new NotSupportedException($"The property name '{definition.Key}' is not supported by '{GetType().Name}'!");
                }
            }
        }

        public void Unlink()
        {
            
        }
    }
}
