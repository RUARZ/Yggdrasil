using System;
using System.Collections.Generic;
using System.Reflection;

namespace Yggdrasil
{
    /// <summary>
    /// Creates a link between a control of a view and methods or properties of a context.
    /// </summary>
    public interface ILinker
    {
        /// <summary>
        /// Creates the link between the <paramref name="control"/> and properties/methods of the <paramref name="context"/>.
        /// </summary>
        /// <param name="control">The control which should be linked with the <paramref name="context"/>.</param>
        /// <param name="context">The context for the control <paramref name="control"/>.</param>
        /// <param name="linkDefinitions">A dictionary with definitions for the link. The key contains the name of the control member and the value contains the matching
        /// name of the context member.</param>
        /// <param name="foundLinks">A dictionary with all found matches. The key is the name of the member of the control and the value contains the <see cref="MemberInfo"/>
        /// of the context.</param>
        /// <param name="createLinkAction">A <see cref="Action"/> for creating a link of sub controls if needed.</param>
        void Link(object control, object context, Dictionary<string, string> linkDefinitions, Dictionary<string, MemberInfo> foundLinks, Action<object, object, string> createLinkAction);

        /// <summary>
        /// Unlinks the view controls to the view model.
        /// </summary
        void Unlink();
    }
}
