using System.Reflection;

namespace Yggdrasil
{
    /// <summary>
    /// Represents data for the <see cref="ILinker"/> implementation.
    /// </summary>
    public class LinkData
    {
        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="LinkData"/>.
        /// </summary>
        /// <param name="viewElementName">The name of the property or event of the view element which should be linked.</param>
        /// <param name="memberInfo">The member Info of the context which should be linked to the view element.</param>
        /// <param name="context">The context to which the view property or element should get linked.</param>
        public LinkData(string viewElementName, MemberInfo memberInfo, object context)
        {
            ViewElementName = viewElementName;
            ContextMemberInfo = memberInfo;
            Context = context;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The name of the property or event of the view which should be linked.
        /// </summary>
        public string ViewElementName { get; }

        /// <summary>
        /// The <see cref="MemberInfo"/> of the context to which the property or event of the view elemtn should be linked.
        /// </summary>
        public MemberInfo ContextMemberInfo { get; }

        /// <summary>
        /// The context for the link.
        /// </summary>
        public object Context { get; }

        #endregion
    }
}
