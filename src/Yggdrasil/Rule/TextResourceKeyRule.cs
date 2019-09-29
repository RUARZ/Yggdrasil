using System;
using System.Text.RegularExpressions;

namespace Yggdrasil
{
    /// <summary>
    /// Represents a rule for getting a text resource key.
    /// </summary>
    class TextResourceKeyRule
    {
        #region Private Consts

        private const string VIEW_NAME_PLACEHOLDER = "<ViewName>";
        private const string VIEW_NAME_GROUP_NAME = "Name";
        private const string CONTROL_NAME_PLACHOLDER = "<ControlName>";
        private const string CONTROL_NAME_GROUP_NAME = "ControlName";

        #endregion

        #region Private Fields

        private readonly string _keyDefinition;
        private readonly Regex _controlNameDefinitionRegex;
        private readonly Regex _viewNameDefinitionRegex;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="TextResourceKeyRule"/> and set its configuration.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> for which this rule applies.</param>
        /// <param name="resourcePropertyName">The name of the property to set a text resource value.</param>
        /// <param name="keyDefinition">The definition for creating a resource key.</param>
        /// <param name="controlNameDefinition">The definition for extracting a name part of the control name. If not defined then the whole control name will be used.</param>
        /// <param name="viewNameDefinition">The definition for extracting a name part of the view name. If not defined then the whole view name will be used.</param>
        public TextResourceKeyRule(Type type, string resourcePropertyName, string keyDefinition, string controlNameDefinition, string viewNameDefinition)
        {
            Type = type;
            ResourcePropertyName = resourcePropertyName;
            _keyDefinition = keyDefinition;
            _controlNameDefinitionRegex = CreateRegex(controlNameDefinition, CONTROL_NAME_PLACHOLDER, CONTROL_NAME_GROUP_NAME);
            _viewNameDefinitionRegex = CreateRegex(viewNameDefinition, VIEW_NAME_PLACEHOLDER, VIEW_NAME_GROUP_NAME);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The <see cref="System.Type"/> for which this <see cref="TextResourceKeyRule"/> is valid.
        /// </summary>
        public Type Type { get; }
        /// <summary>
        /// The property name for setting a text resource value.
        /// </summary>
        public string ResourcePropertyName { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get's a resource key based on the configuration.
        /// </summary>
        /// <param name="viewType">The <see cref="System.Type"/> of the view for getting the name.</param>
        /// <param name="controlName">The name of the control.</param>
        /// <returns>The resource key.</returns>
        public string GetResourceKey(Type viewType, string controlName)
        {
            return _keyDefinition
                .Replace(VIEW_NAME_PLACEHOLDER, GetNamePart(viewType.Name, _viewNameDefinitionRegex, VIEW_NAME_GROUP_NAME))
                .Replace(CONTROL_NAME_PLACHOLDER, GetNamePart(controlName, _controlNameDefinitionRegex, CONTROL_NAME_GROUP_NAME));
        }

        #endregion

        #region Private Methods

        private Regex CreateRegex(string definition, string placeholder, string groupName)
        {
            if (string.IsNullOrEmpty(definition))
                return null;

            string regexPattern = definition
                .Replace(placeholder, $"(?<{groupName}>\\w+)");

            return new Regex(regexPattern);
        }

        private string GetNamePart(string name, Regex regex, string regexGroupName)
        {
            if (name == null)
                return string.Empty;

            string result = null;

            if (regex != null)
            {
                Match m = regex.Match(name);

                if (m.Success)
                    result = m.Groups[regexGroupName].Value;
            }

            return result ?? name;
        }

        #endregion
    }
}
