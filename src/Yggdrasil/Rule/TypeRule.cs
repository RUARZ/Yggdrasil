using System;
using System.Text.RegularExpressions;

namespace Yggdrasil
{
    /// <summary>
    /// Class for searching for the matching type name defined from the type definitions.
    /// </summary>
    class TypeRule
    {
        #region Private Consts

        private const string NAMESPACE_GROUP_NAME = "Namespace";
        private const string BASE_NAME_GROUP_NAME = "Basename";

        #endregion

        #region Private Fields

        private readonly string _resultTypeNameDefinition;
        private readonly Regex _originTypeNameRegex;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="TypeRule"/> and sets the definitions for locating type names.
        /// </summary>
        /// <param name="originTypeDefinition">Definition of the origin type name.</param>
        /// <param name="viewTypeNameDefinition">Definition of the result type name.</param>
        public TypeRule(string originTypeDefinition, string resultTypeDefinition)
        {
            _originTypeNameRegex = CreateTypeNameRegex(originTypeDefinition);
            _resultTypeNameDefinition = resultTypeDefinition;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if the passed <paramref name="type"/> matches the definition of origin type name and returns <see langword="true"/> if so.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> which should be checked against the origin type name definition.</param>
        /// <returns><see langword="True"/> if the type name matches the origin type definition, otherwise <see langword="false"/> will be returned.</returns>
        public bool IsTypeNameMatching(Type type)
        {
            return _originTypeNameRegex.IsMatch(type.FullName);
        }

        /// <summary>
        /// Get's the full name for the searched result type name based on the definition of the type rule.
        /// </summary>
        /// <param name="type">The origin type to create the result type name based on the rule.</param>
        /// <returns>The result type name based on the rule.</returns>
        /// <exception cref="Exception">Thrown if the full name of the <paramref name="type"/> does not match the type name definition for origin.</exception>
        public string GetResultTypeName(Type type)
        {
            if (!_originTypeNameRegex.IsMatch(type.FullName))
                throw new Exception("Type name not valid!");//TODO: new exception and better message!

            Match match = _originTypeNameRegex.Match(type.FullName);

            return _resultTypeNameDefinition
                .Replace(RuleProvider.NAMESPACE_PLACEHOLDER, match.Groups[NAMESPACE_GROUP_NAME].Value)
                .Replace(RuleProvider.BASE_NAME_PLACEHOLDER, match.Groups[BASE_NAME_GROUP_NAME].Value)
                + ", " + type.Assembly.FullName;
        }

        #endregion

        #region Private Methods

        private Regex CreateTypeNameRegex(string typeNameDefinition)
        {
            string regexPattern = typeNameDefinition
                .Replace(RuleProvider.NAMESPACE_PLACEHOLDER, $"(?<{NAMESPACE_GROUP_NAME}>[a-zA-Z]\\w+(?:\\.[a-zA-Z]\\w+)*)")
                .Replace(RuleProvider.BASE_NAME_PLACEHOLDER, $"(?<{BASE_NAME_GROUP_NAME}>\\w+)");

            return new Regex(regexPattern);
        }

        #endregion
    }
}
