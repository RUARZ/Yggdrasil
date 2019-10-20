using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Yggdrasil.Culture;
using Yggdrasil.Resource;

namespace Yggdrasil
{
    /// <summary>
    /// Represents a display item with its view and viewmodel.
    /// </summary>
    class DisplayItem : IDisposable
    {
        #region Private Methods

        private readonly List<ILinker> _linkers = new List<ILinker>();
        private readonly List<(FieldInfo FieldInfo, TextResourceKeyRule KeyRule, PropertyInfo PropertyInfo)> _resourceFields = new List<(FieldInfo FieldInfo, TextResourceKeyRule KeyRule, PropertyInfo PropertyInfo)>();
        private (TextResourceKeyRule KeyRule, PropertyInfo PropertyInfo)? _viewResourceField;
        private CultureChangedSubscription _cultureChangedSubscription;

        #endregion

        #region Consturctor

        /// <summary>
        /// Creates a new instance of <see cref="DisplayItem"/> and set's its view and viewmodel.
        /// </summary>
        /// <param name="view">The view for the <see cref="DisplayItem"/>.</param>
        /// <param name="viewModel">The view model for the <see cref="DisplayItem"/>.</param>
        public DisplayItem(object view, object viewModel)
        {
            View = view;
            ViewModel = viewModel;
            SetFieldActions();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The displayed view.
        /// </summary>
        public object View { get; }
        /// <summary>
        /// The displayed view model which is set to <see cref="View"/>.
        /// </summary>
        public object ViewModel { get; }

        #endregion

        #region Public Methods

        public void Dispose()
        {
            if (View is IDisposable disposableView)
                disposableView.Dispose();

            if (ViewModel is IDisposable disposableViewModel)
                disposableViewModel.Dispose();

            _cultureChangedSubscription?.Dispose();

            foreach (ILinker linker in _linkers)
            {
                linker.Unlink();
            }
        }

        #endregion

        #region Private Methods

        private void SetFieldActions()
        {
            if (!RuleProvider.AreLinkRulesDefined && !RuleProvider.AreTextResourceKeyRulesDefined)
                return;

            foreach (FieldInfo viewFieldInfo in View.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (RuleProvider.AreLinkRulesDefined)
                    AddLink(viewFieldInfo);

                if (RuleProvider.AreTextResourceKeyRulesDefined)
                    AddResourceFields(viewFieldInfo);
            }

            if (RuleProvider.AreTextResourceKeyRulesDefined)
            {
                TextResourceKeyRule rule = RuleProvider.GetTextResourceKeyRule(View.GetType());

                if (rule != null)
                {
                    PropertyInfo info = View.GetType().GetProperty(rule.ResourcePropertyName);

                    if (info != null)
                        _viewResourceField = (rule, info);
                }

                _cultureChangedSubscription = CultureManager.Subscribe(info => SetResourceValues());
                SetResourceValues();
            }
        }

        private void AddLink(FieldInfo viewFieldInfo)
        {
            List<LinkRule> linkRules = RuleProvider.GetLinkRulesForType(viewFieldInfo.FieldType);

            if (linkRules.Count <= 0)
                return;

            ILinker linker = LinkerManager.GetLinkerForType(viewFieldInfo.FieldType);

            if (linker == null)
                return;

            Link(viewFieldInfo.GetValue(View), ViewModel, linkRules, linker, viewFieldInfo.Name);
        }

        private void CreateLink(object control, object context, string controlName)
        {
            List<LinkRule> linkRules = RuleProvider.GetLinkRulesForType(control.GetType());

            if (linkRules.Count <= 0)
                return;

            ILinker linker = LinkerManager.GetLinkerForType(control.GetType());

            if (linker == null)
                return;

            Link(control, context, linkRules, linker, controlName);
        }

        private void Link(object control, object context, IEnumerable<LinkRule> linkRules, ILinker linker, string controlName)
        {
            Type contextType = context?.GetType();

            Dictionary<string, MemberInfo> foundLinks = new Dictionary<string, MemberInfo>();
            Dictionary<string, string> linkDefinitions = new Dictionary<string, string>();

            foreach (LinkRule rule in linkRules)
            {
                MemberInfo info = null;
                string linkInfoName = rule.GetLinkInfoName(controlName);

                switch (rule.RuleType)
                {
                    case LinkRuleType.Property:
                        info = !string.IsNullOrEmpty(linkInfoName) ? contextType?.GetProperty(linkInfoName) : null;
                        break;
                    case LinkRuleType.Event:
                        info = contextType?.GetMethod(linkInfoName);
                        break;
                }

                linkDefinitions.Add(rule.InfoName, linkInfoName);
                if (info != null)
                    foundLinks.Add(rule.InfoName, info);
            }

            _linkers.Add(linker);
            linker.Link(control, context, linkDefinitions, foundLinks, CreateLink);
        }

        private void AddResourceFields(FieldInfo viewFieldInfo)
        {
            TextResourceKeyRule rule = RuleProvider.GetTextResourceKeyRule(viewFieldInfo.FieldType);

            if (rule == null)
                return;

            PropertyInfo propInfo = viewFieldInfo.FieldType.GetProperty(rule.ResourcePropertyName);

            if (propInfo == null)
                return;

            _resourceFields.Add((viewFieldInfo, rule, propInfo));
        }

        private void SetResourceValues()
        {
            if (_viewResourceField != null)
            {
                _viewResourceField.Value.PropertyInfo
                    .SetValue(View,
                    ResourceHandler.GetResource(_viewResourceField.Value.KeyRule.GetResourceKey(View.GetType(), null), true));
            }

            foreach (var res in _resourceFields)
            {
                SetResource(res.FieldInfo, res.PropertyInfo, ResourceHandler.GetResource(res.KeyRule.GetResourceKey(View.GetType(), res.FieldInfo.Name), true));
            }
        }

        private void SetResource(FieldInfo fieldInfo, PropertyInfo propInfo, string text)
        {
            // if the text is empty then there was no resource found and then do not set the resource!
            if (string.IsNullOrEmpty(text))
                return;

            object field = fieldInfo.GetValue(View);

            if (field == null)
                return;

            propInfo.SetValue(field, text);
        }

        #endregion
    }
}
