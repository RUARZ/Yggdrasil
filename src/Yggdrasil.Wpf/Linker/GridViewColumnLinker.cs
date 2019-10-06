using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(GridViewColumn))]
    public class GridViewColumnLinker : ILinker
    {
        #region Interface Implementation

        public void Link(object control, object context, Dictionary<string, string> linkDefinitions, Dictionary<string, MemberInfo> foundLinks, Action<object, object, string> createLinkAction)
        {
            if (!(control is GridViewColumn gridViewColumn))
                return;

            foreach(KeyValuePair<string, string> definition in linkDefinitions)
            {
                switch(definition.Key)
                {
                    case nameof(GridViewColumn.DisplayMemberBinding):
                        Binding binding = new Binding(definition.Value);
                        binding.Source = context;
                        gridViewColumn.DisplayMemberBinding = binding;
                        break;
                    default:
                        throw new NotSupportedException($"Property '{definition.Key}' is not supported by {GetType().Name}!");
                }
            }
        }

        public void Unlink()
        {
            
        }

        #endregion
    }
}
