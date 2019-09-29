using System;
using System.Collections.Generic;
using System.Text;

namespace Yggdrasil
{
    public interface IViewModelMapper
    {
        object GetModel(object view);
    }
}