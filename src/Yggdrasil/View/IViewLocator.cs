using System;
using System.Collections.Generic;
using System.Text;

namespace Yggdrasil
{
    public interface IViewLocator
    {
        object GetView(object model);
    }
}