using System;
using System.Collections.Generic;
using System.Text;

namespace Yggdrasil
{
    public interface IDirtyStateModel
    {
        event EventHandler DirtyStateChanged;

        bool IsDirty { get; set; }
    }
}