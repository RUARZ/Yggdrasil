using System;
using System.Collections.Generic;

namespace Yggdrasil
{
    /// <summary>
    /// Creates a link between a control of a view and methods or properties of a context.
    /// </summary>
    public interface ILinker
    {
        /// <summary>
        /// Creates the link between the <paramref name="viewElement"/> and properties/methods of the <paramref name="context"/>.
        /// </summary>
        /// <param name="viewElement">The control which should be linked with the <paramref name="context"/>.</param>
        /// <param name="linkData"><see cref="IEnumerable{T}"/> of <see cref="LinkData"/> which provides the data to create links.</param>
        /// <param name="createLinkAction">A <see cref="Action"/> for creating a link of sub controls if needed.</param>
        void Link(object viewElement, IEnumerable<LinkData> linkData, Action<object, object, string> createLinkAction);

        /// <summary>
        /// Unlinks the view controls to the view model.
        /// </summary
        void Unlink();
    }
}
