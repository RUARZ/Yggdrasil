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

            EnumerateFields(view.GetType(), info => CreateLink(info.GetValue(view), context, info.Name, view));
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
            CreateLink(control, context, controlName, null);
        }

        private void CreateLink(object control, object context, string controlName, object view)
        {
            if (control == null)
                return;

            IReadOnlyList<LinkRule> linkRules = RuleProvider.GetLinkRulesForType(control.GetType());

            if (linkRules.Count <= 0)
                return;

            ILinker linker = LinkerManager.GetLinkerForType(control.GetType());

            if (linker == null)
                return;

            Link(control, context, linkRules, linker, controlName, view);
        }

        private void Link(object control, object context, IEnumerable<LinkRule> linkRules, ILinker linker, string controlName, object view)
        {
            Type contextType = context?.GetType();

            List<LinkData> linkData = new List<LinkData>();
            
            foreach (LinkRule rule in linkRules)
            {
                LinkData data = CreateLinkData(controlName, rule, context.GetType(), context, rule.InfoName, null, view);

                if (data != null)
                {
                    linkData.Add(data);
                }
            }

            //if no data found to link then return and don't call / add the linker
            if (linkData.Count == 0)
                return;

            _linkers.Add(linker);
            linker.Link(control, linkData, CreateLink);
        }

        private LinkData CreateLinkData(string controlName, LinkRule rule, Type type, object context, string viewElementInfoName, List<PropertyInfo> propertyPath, object view)
        {
            string[] nameParts = RuleProvider.GetNameSeparatorFunc()?.Invoke(controlName ?? string.Empty) ?? null;

            if (nameParts == null || nameParts.Length == 1)
            {
                if (string.IsNullOrEmpty(controlName))
                    return null;

                MemberInfo info = type.GetMember(rule.GetLinkInfoName(controlName), BindingFlags.Instance | BindingFlags.Public).FirstOrDefault();

                // if there was already a match found then create a LinkData and return it
                if (info != null)
                    return new LinkData(viewElementInfoName, info, context, propertyPath);

                propertyPath = new List<PropertyInfo>();

                // if no match found then search within all properties of the passed context which are a class itself (except enumerables)
                LinkData data = CreateLinkDataForSubProperties(type, context, rule.GetLinkInfoName(controlName), viewElementInfoName, propertyPath, rule);

                if (data != null)
                    data.PropertyPath = data.PropertyPath?.Reverse();
                return data;
            }
            else
            {
                propertyPath = new List<PropertyInfo>();
                Type currentType = type;
                object lastControl = null;
                for (int i = 0; i < nameParts.Length; i++)
                {
                    // if the last of the name parts is reached then try to create the link data with the collected infos.
                    if (i == nameParts.Length - 1)
                        return CreateLinkData(nameParts[i], rule, currentType, context, viewElementInfoName, propertyPath, view);

                    // if no last control is set then first try to find the control which is defined in this name part.
                    if (lastControl == null)
                    {
                        EnumerateFields(view.GetType(), info =>
                        {
                            if (info.Name == nameParts[i])
                                lastControl = info.GetValue(view);
                        });
                    }
                    else
                    {
                        // get the context property name for further search based on the last control and the current name part which must define the property of the control 
                        // to get the context property name for further search.
                        string contextPropertyName = RuleProvider.GetContextPropertyNameForTypeAndProperty(lastControl.GetType(), nameParts[i], nameParts[i - 1]);
                        // get the property info for the context property and add it to the property path. Just assume that the property is found. It's ok if a exception occurs
                        // if the property could not be found because of e.g. typos.
                        PropertyInfo pInfo = currentType.GetProperty(contextPropertyName, BindingFlags.Instance | BindingFlags.Public);
                        propertyPath.Add(pInfo);
                        // set the current type for the next search or link creation!
                        currentType = pInfo.PropertyType;
                        lastControl = null;
                    }
                }
            }

            return null;
        }

        private LinkData CreateLinkDataForSubProperties(Type type, object context, string contextInfoName, string viewElementInfoName, List<PropertyInfo> propertyPath, LinkRule rule)
        {
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
                else if (pInfo.GetIndexParameters().Length <= 0)
                {
                    subContext = pInfo.GetValue(context);
                    subType = pInfo.PropertyType;
                }
                else
                {
                    subContext = null;
                    subType = pInfo.PropertyType;
                }

                LinkData data = CreateLinkData(null, rule, subType, subContext, viewElementInfoName, propertyPath, null);

                // if a match found in the sub class then return the link data.
                if (data != null)
                {
                    propertyPath.Add(pInfo);
                    return data;
                }
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
