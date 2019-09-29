using System;
using System.Collections.Generic;
using System.Linq;
using Yggdrasil.Exceptions;

namespace Yggdrasil
{
    /// <summary>
    /// Provides methods for defining/adding different rules.
    /// </summary>
    public static class RuleProvider
    {
        #region Public Consts

        public const string NAMESPACE_PLACEHOLDER = "<Namespace>";
        public const string BASE_NAME_PLACEHOLDER = "<Basename>";

        #endregion

        #region Private Fields

        private static readonly List<TypeRule> _typeRules = new List<TypeRule>();
        private static readonly Dictionary<Type, List<LinkRule>> _linkRules = new Dictionary<Type, List<LinkRule>>();
        private static readonly Dictionary<Type, TextResourceKeyRule> _textResourceKeyRules = new Dictionary<Type, TextResourceKeyRule>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Information if <see cref="LinkRule"/>s where added.
        /// </summary>
        internal static bool AreLinkRulesDefined => _linkRules.Count > 0;
        /// <summary>
        /// Information if <see cref="TextResourceKeyRule"/>s where added.
        /// </summary>
        internal static bool AreTextResourceKeyRulesDefined => _textResourceKeyRules.Count > 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a definition for view model and view type names to create type name for views based on view model type name.
        /// </summary>
        /// <param name="viewModelTypeNameDefinition">Definition of the type name for view models.</param>
        /// <param name="viewTypeNameDefinition">Definition of the type name for views resulting from passed view model types.</param>
        public static void AddTypeRule(string viewModelTypeNameDefinition, string viewTypeNameDefinition)
        {
            _typeRules.Add(new TypeRule(viewModelTypeNameDefinition, viewTypeNameDefinition));
        }

        /// <summary>
        /// Adds a definition for creating a link between a control property, event, .. to a view model property, event, method, ...
        /// </summary>
        /// <param name="controlType"><see cref="Type"/> of the control for which the link is for.</param>
        /// <param name="ruleType">The type of the rule.</param>
        /// <param name="controlInfoName">The name of the controls event, property, .. for the link.</param>
        /// <param name="viewModelInfoNameDefinition">The definition of the resulting info name of the view model. Use '<Name>' as placeholder for the control name.</param>
        public static void AddLinkRule(Type controlType, LinkRuleType ruleType, string controlInfoName, string viewModelInfoNameDefinition)
        {
            if (!_linkRules.ContainsKey(controlType))
                _linkRules.Add(controlType, new List<LinkRule>());

            _linkRules[controlType].Add(new LinkRule(controlInfoName, viewModelInfoNameDefinition, ruleType));
        }

        /// <summary>
        /// Adds a definition for creating a link between a control property, event, .. to a view model property, event, method, ...
        /// </summary>
        /// <param name="controlType"><see cref="Type"/> of the control for which the link is for.</param>
        /// <param name="ruleType">The type of the rule.</param>
        /// <param name="controlInfoName">The name of the controls event, property, .. for the link.</param>
        /// <param name="controlNameDefinition">The definition of the name of the control.</param>
        /// <param name="viewModelInfoNameDefinition">The definition of the resulting info name of the view model. Use '<Name>' as placeholder for the control name.</param>
        public static void AddLinkRule(Type controlType, LinkRuleType ruleType, string controlInfoName, string controlNameDefinition, string viewModelInfoNameDefinition)
        {
            if (!_linkRules.ContainsKey(controlType))
                _linkRules.Add(controlType, new List<LinkRule>());

            _linkRules[controlType].Add(new LinkRule(controlInfoName, controlNameDefinition, viewModelInfoNameDefinition, ruleType));
        }

        /// <summary>
        /// Adds a text resource key rule for localizing text resources.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the control for this rule.</param>
        /// <param name="resourcePropertyName">The name of the property to which the text resource should be applied.</param>
        /// <param name="keyDefinition">Definition of the key value creation.</param>
        /// <param name="controlNameDefinition">Definition for getting a part out of the control name. If not defined then the entire control name will be used.</param>
        /// <param name="viewNameDefinition">Definition for getting a part out of the view name for the key. If not defined then the entire name of the view will be used 
        /// (if defined within <paramref name="keyDefinition"/>).</param>
        public static void AddTextResourceKeyRule(Type type, string resourcePropertyName, string keyDefinition, string controlNameDefinition = null, string viewNameDefinition = null)
        {
            _textResourceKeyRules.Add(type, new TextResourceKeyRule(type, resourcePropertyName, keyDefinition, controlNameDefinition, viewNameDefinition));
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Get's the <see cref="Type"/> of the view for the passed <paramref name="modelType"/> based on added definitions.
        /// </summary>
        /// <param name="modelType"><see cref="Type"/> of the view model for which the <see cref="Type"/> of the belonging view should be returned.</param>
        /// <returns>The <see cref="Type"/> of the view which belongs to the <paramref name="modelType"/> based on the definitions.</returns>
        /// <exception cref="NoMatchingTypeFoundException">Thrown if no matching view was found!</exception>
        internal static Type GetViewType(Type modelType)
        {
            foreach (TypeRule rule in _typeRules)
            {
                if (!rule.IsTypeNameMatching(modelType))
                    continue;

                string typeName = rule.GetResultTypeName(modelType);

                return Type.GetType(typeName);
            }

            throw new NoMatchingTypeFoundException($"No matching view type found for model type '{modelType.FullName}'!");
        }

        /// <summary>
        /// Get's the added <see cref="LinkRule"/>s for the passed <paramref name="type"/>. If no <see cref="LinkRule"/> is defined then a emtpy
        /// list will be returned.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which the <see cref="LinkRule"/>s should be retrieved.</param>
        /// <returns>The defined <see cref="LinkRule"/>s for the passed <paramref name="type"/>. If no <see cref="LinkRule"/> is defined then a empty
        /// list will be returend.</returns>
        internal static List<LinkRule> GetLinkRulesForType(Type type)
        {
            if (_linkRules.ContainsKey(type))
                return _linkRules[type];

            return _linkRules.FirstOrDefault(x => x.Key.IsAssignableFrom(type)).Value ?? new List<LinkRule>();
        }

        /// <summary>
        /// Get's the <see cref="TextResourceKeyRule"/> for the passed <paramref name="controlType"/>. If no rule defined then <see langword="null"/>
        /// will be returned.
        /// </summary>
        /// <param name="controlType">The <see cref="Type"/> of the control for retreiving the rule.</param>
        /// <returns>The rule for the passed <paramref name="controlType"/>, if no rule was defined then <see langword="null"/> will be returned.</returns>
        internal static TextResourceKeyRule GetTextResourceKeyRule(Type controlType)
        {
            if (_textResourceKeyRules.ContainsKey(controlType))
                return _textResourceKeyRules[controlType];

            return _textResourceKeyRules.FirstOrDefault(x => x.Key.IsAssignableFrom(controlType)).Value;
        }

        #endregion
    }
}
