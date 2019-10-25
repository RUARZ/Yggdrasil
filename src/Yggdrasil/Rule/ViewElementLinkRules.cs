using System.Collections.Generic;

namespace Yggdrasil
{
    /// <summary>
    /// Contains link rules for a view element type.
    /// </summary>
    class ViewElementLinkRules
    {
        #region Private Fields

        private readonly string _viewElementNameDefinition;
        private readonly List<LinkRule> _linkRules = new List<LinkRule>();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="ViewElementLinkRules"/> and set's the definition of getting the name part of the view element name.
        /// </summary>
        /// <param name="viewElementNameDefinition">The definition for getting the base name from a view element name.</param>
        public ViewElementLinkRules(string viewElementNameDefinition)
        {
            _viewElementNameDefinition = viewElementNameDefinition;
        }

        #endregion

        #region Public Properties

        public IReadOnlyList<LinkRule> Rules => _linkRules;

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a <see cref="LinkRule"/>.
        /// </summary>
        /// <param name="viewElementPropertyName">The name of the property/event/... from the view element.</param>
        /// <param name="contextNameDefinition">The definition of the property/method/... - name from the context to create the link.</param>
        public void Add(string viewElementName, string contextNameDefinition)
        {
            _linkRules.Add(new LinkRule(viewElementName, _viewElementNameDefinition, contextNameDefinition));
        }

        #endregion
    }
}
