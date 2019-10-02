using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(Label))]
    public class LabelLinker : ILinker
    {
        #region Interface Implementation

        public void Link(object control, object context, Dictionary<string, string> linkDefinitions, Dictionary<string, MemberInfo> foundLinks, Action<object, object, string> createLinkAction)
        {
            if (!(control is Label label))
                return;

            foreach(KeyValuePair<string, MemberInfo> definition in foundLinks)
            {
                switch (definition.Key)
                {
                    case nameof(Label.Content):
                        Binding binding = new Binding(definition.Value.Name);
                        binding.Source = context;
                        BindingOperations.SetBinding(label, ContentControl.ContentProperty, binding);
                        break;
                    default:
                        throw new NotSupportedException($"Property '{definition.Key}' is not supported by {GetType().Name}!");
                }
            }
        }

        public void Unlink()
        {
            // nothing to do!
        }

        #endregion
    }
}
