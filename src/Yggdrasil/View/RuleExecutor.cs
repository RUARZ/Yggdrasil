using System;
using System.Collections.Generic;
using System.Reflection;
using Yggdrasil.Resource;

namespace Yggdrasil
{
    /// <summary>
    /// Class for searching matching rules between view and view model or textresource rules...
    /// Creates found links, set's resources...
    /// </summary>
    class RuleExecutor : IDisposable
    {
        #region Private Fields

        private readonly List<ILinker> _linkers = new List<ILinker>();

        #endregion

        #region Public Methods

        public void CreateLinks(object view, object context)
        {
            //if no rules defined for links --> nothing to do
            if (!RuleProvider.AreLinkRulesDefined)
                return;

            EnumerateFields(view.GetType(), info => CreateLink(info.GetValue(view), context, info.Name));
        }

        public void SetResources(object view)
        {
            //if no rules defined for textresources --> nothing to do
            if (!RuleProvider.AreTextResourceKeyRulesDefined)
                return;

            Type viewType = view.GetType();
            TextResourceKeyRule rule = RuleProvider.GetTextResourceKeyRule(viewType);

            if (rule != null)
            {
                SetResource(viewType, view, null, viewType);
            }

            EnumerateFields(viewType, info =>
            {
                SetResource(info.FieldType, info.GetValue(view), info.Name, viewType);
            });
        }

        #endregion

        #region Private Methods

        private void EnumerateFields(Type type, Action<FieldInfo> fieldAction)
        {
            foreach (FieldInfo info in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                fieldAction(info);
            }
        }

        private void CreateLink(object control, object context, string controlName)
        {
            if (control == null)
                return;

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

        //TODO continue implementing to link properties in sub classes if the wished property could not be found in 
        //the passed context.
        //private MemberInfo GetMemberInfo(object context, LinkRuleType linkType, string infoName)
        //{
        //    MemberInfo info = null;

        //    switch (linkType)
        //    {
        //        case LinkRuleType.Property:
        //            break;
        //        case LinkRuleType.Event:
        //            break;
        //    }


        //}

        private void SetResource(Type type, object control, string controlName, Type viewType)
        {
            TextResourceKeyRule rule = RuleProvider.GetTextResourceKeyRule(type);

            if (rule == null)
                return;

            PropertyInfo pInfo = type.GetProperty(rule.ResourcePropertyName);

            if (pInfo == null)
                return;

            pInfo.SetValue(control, ResourceHandler.GetResource(rule.GetResourceKey(viewType, controlName), true));
        }

        #endregion

        #region Interface Implementation

        public void Dispose()
        {
            foreach (ILinker linker in _linkers)
            {
                linker.Unlink();
            }
        }

        #endregion
    }
}
