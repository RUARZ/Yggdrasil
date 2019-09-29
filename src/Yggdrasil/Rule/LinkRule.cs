using System.Text.RegularExpressions;

namespace Yggdrasil
{
    /// <summary>
    /// Class for searching for matching property-, event- or method names depending on defined rules.
    /// </summary>
    class LinkRule
    {
        #region Private Consts

        private const string NAME_PLACEHOLDER_NAME = "<Name>";
        private const string NAME_GROUP_NAME = "Name";

        #endregion

        #region Private Fields

        private readonly string _resultInfoNameDefinition;
        private readonly Regex _controlNameRegex;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="LinkRule"/> and sets its definitions.
        /// </summary>
        /// <param name="infoName">The name of the property, method, event... of the control which should be linked to the view model.</param>
        /// <param name="resultNameDefinition">The definition of the name of the property, method, event... of the view model for linking.</param>
        /// <param name="ruleType">The type of the rule.</param>
        public LinkRule(string infoName, string resultNameDefinition, LinkRuleType ruleType)
        {
            InfoName = infoName;
            _resultInfoNameDefinition = resultNameDefinition;
            RuleType = ruleType;
        }

        /// <summary>
        /// Creates a new instance of <see cref="LinkRule"/> and sets its definitions.
        /// </summary>
        /// <param name="infoName">The name of the property, method, event... of the control which should be linked to the view model.</param>
        /// <param name="controlNameDefinition">Definition of the naming of the control.</param>
        /// <param name="resultNameDefinition">The definition of the name of the property, method, event... of the view model for linking.</param>
        /// <param name="ruleType">The type of the rule.</param>
        public LinkRule(string infoName, string controlNameDefinition, string resultNameDefinition, LinkRuleType ruleType)
        {
            InfoName = infoName;
            _resultInfoNameDefinition = resultNameDefinition;
            RuleType = ruleType;
            _controlNameRegex = CreateControlNameRegex(controlNameDefinition);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The name of the property, method, event... of the control which should be linked to the view model.
        /// </summary>
        public string InfoName { get; }
        /// <summary>
        /// The type of the rule.
        /// </summary>
        public LinkRuleType RuleType { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the link info name of the view model for the passed control name.
        /// </summary>
        /// <param name="controlName">The name of the control for searching the link info name.</param>
        /// <returns>The link info name for the passed <paramref name="controlName"/>.</returns>
        public string GetLinkInfoName(string controlName)
        {
            if (_controlNameRegex == null)
                return _resultInfoNameDefinition.Replace(NAME_PLACEHOLDER_NAME, controlName);

            Match m = _controlNameRegex.Match(controlName);
            string name = m.Groups[NAME_GROUP_NAME].Value;

            return _resultInfoNameDefinition.Replace(NAME_PLACEHOLDER_NAME, name);
        }

        #endregion

        #region Private Methods

        private Regex CreateControlNameRegex(string controlNameDefinition)
        {
            string regexPattern = controlNameDefinition
                .Replace(NAME_PLACEHOLDER_NAME, $"(?<{NAME_GROUP_NAME}>\\w+)");

            return new Regex(regexPattern);
        }

        #endregion
    }
}
