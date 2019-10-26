using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Creates the links between the passed <see cref="view"/> and <paramref name="context"/>.
        /// </summary>
        /// <param name="view">The view for searching the fields and creating the links.</param>
        /// <param name="context">The context to link with the view fields.</param>
        public void CreateLinks(object view, object context)
        {
            //if no rules defined for links --> nothing to do
            if (!RuleProvider.AreLinkRulesDefined)
                return;

            EnumerateFields(view.GetType(), info => CreateLink(info.GetValue(view), context, info.Name));
        }

        /// <summary>
        /// Set's the resources based on the reource rules for the passed <paramref name="view"/>.
        /// </summary>
        /// <param name="view">The view on which the defined resources should be set.</param>
        public void SetResources(object view)
        {
            //if no rules defined for textresources --> nothing to do
            if (!RuleProvider.AreTextResourceKeyRulesDefined)
                return;

            //first check for the view itself if a resource was defined which should be set.
            Type viewType = view.GetType();
            TextResourceKeyRule rule = RuleProvider.GetTextResourceKeyRule(viewType);

            if (rule != null)
            {
                SetResource(viewType, view, null, viewType);
            }

            // second enumerate through the fields of the view to set the resources.
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

            IReadOnlyList<LinkRule> linkRules = RuleProvider.GetLinkRulesForType(control.GetType());

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

            List<LinkData> linkData = new List<LinkData>();
            
            foreach (LinkRule rule in linkRules)
            {
                LinkData data = CreateLinkData(rule.GetLinkInfoName(controlName), context.GetType(), context, rule.InfoName);

                if (data != null)
                    linkData.Add(data);
            }

            //if no data found to link then return and don't call / add the linker
            if (linkData.Count == 0)
                return;

            _linkers.Add(linker);
            linker.Link(control, linkData, CreateLink);
        }

        private LinkData CreateLinkData(string contextInfoName, Type type, object context, string viewElementInfoName)
        {
            MemberInfo info = type.GetMember(contextInfoName, BindingFlags.Instance | BindingFlags.Public).FirstOrDefault();
            
            // if there was already a match found then create a LinkData and return it
            if (info != null)
                return new LinkData(viewElementInfoName, info, context);

            // if no match found then search within all properties of the passed context which are a class itself (except enumerables)
            foreach (PropertyInfo pInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!pInfo.PropertyType.IsClass || pInfo.PropertyType == typeof(string))
                    continue;

                object subContext;
                Type subType;

                if (typeof(IEnumerable).IsAssignableFrom(pInfo.PropertyType))
                {
                    // if the type is a ienurable type then set the context to null because then the linker should be only informed
                    // about the found members in the generic used class
                    subContext = null;
                    subType = pInfo.PropertyType.GetGenericArguments().First();
                }
                else
                {
                    subContext = pInfo.GetValue(context);
                    subType = pInfo.PropertyType;
                }

                LinkData data = CreateLinkData(contextInfoName, subType, subContext, viewElementInfoName);

                // if a match found in the sub class then return the link data.
                if (data != null)
                    return data;
            }

            return null;
        }

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
