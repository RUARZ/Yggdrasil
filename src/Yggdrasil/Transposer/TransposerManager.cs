using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Yggdrasil.Transposer
{
    /// <summary>
    /// Static class which provides methods for activating transposer.
    /// </summary>
    static class TransposerManager
    {
        #region Private Fields

        /// <summary>
        /// Cache of the current used transposer.
        /// </summary>
        private static readonly Dictionary<object, TransposerEntry> _cache = new Dictionary<object, TransposerEntry>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Enable transposers for the passed <paramref name="view"/> and calls <see cref="ITransposer.OnTie(object, object)"/>.
        /// </summary>
        /// <param name="view">The view for which a transposer should be enabled.</param>
        public static void Enable(object view)
        {
            EnableTransposer(_cache, view, GetTransposer(view));
        }

        /// <summary>
        /// Disables a transposer from the passed <paramref name="view"/> and calls <see cref="ITransposer.OnUntie(object, object)"/>.
        /// </summary>
        /// <param name="view">The view for which a transposer should be disabled.</param>
        public static void Disable(object view)
        {
            DisableTransposer(_cache, view);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a transposer for the passed <paramref name="view"/> if there is any defined / should be used.
        /// If no transposer defined then <see langword="null"/> will be returned.
        /// </summary>
        /// <param name="view">The view for searching a implementation of <see cref="ITransposer"/>.</param>
        /// <returns>A instance which implements <see cref="ITransposer"/> if any is defined for the <paramref name="view"/>, otherwise <see langword="null"/>.</returns>
        private static ITransposer GetTransposer(object view)
        {
            // enumerate through the app domain and search all classes which are marked with the 'RegisterTranspsoerAttribute'
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes().Where(x => x is ITransposer))
                {
                    RegisterTransposerAttribute att = type.GetCustomAttribute<RegisterTransposerAttribute>();
                    if (att == null)
                        continue;

                    if (att.ViewType == view.GetType())
                        return Activator.CreateInstance(type) as ITransposer;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the context from the passed <paramref name="view"/>.
        /// </summary>
        /// <param name="view">The view from which the context should get retrieved.</param>
        /// <returns>The context of the passed <paramref name="view"/>.</returns>
        private static object GetContextFromView(object view)
        {
            return ViewManager.GetContextOfView(view);
        }

        /// <summary>
        /// Enables a transposer to the passed <paramref name="cache"/>. If there is already a transposer enabled then the old one will be removed and <see cref="ITransposer.OnUntie(object, object)"/>
        /// will be called.
        /// </summary>
        /// <param name="cache">The cache for enabling a transposer.</param>
        /// <param name="view">The view for the transposer.</param>
        /// <param name="transposer">The transposer which should be enabled.</param>
        private static void EnableTransposer(Dictionary<object, TransposerEntry> cache, object view, ITransposer transposer)
        {
            if (cache.TryGetValue(view, out TransposerEntry entry))
            {
                object viewModel = entry.ViewModel;
                transposer = entry.Transposer;
                transposer?.OnUntie(view, viewModel);
                cache.Remove(view);
            }

            if (transposer == null)
                return;

            object context = GetContextFromView(view);
            transposer.OnTie(view, context);

            cache[view] = new TransposerEntry(transposer, context);
        }

        /// <summary>
        /// Disables a transposer for the passed <paramref name="view"/> from the passed <paramref name="cache"/>.
        /// </summary>
        /// <param name="cache">The transposer cache for disabling the transposer.</param>
        /// <param name="view">The view for disabling the transposer.</param>
        private static void DisableTransposer(Dictionary<object, TransposerEntry> cache, object view)
        {
            if (!cache.TryGetValue(view, out TransposerEntry entry))
                return;

            entry.Transposer?.OnUntie(view, GetContextFromView(view));
            cache.Remove(view);
        }
             
        #endregion

        #region Private Class TransposerEntry

        /// <summary>
        /// Represents a entry for a used transposer.
        /// </summary>
        private class TransposerEntry
        {
            public TransposerEntry(ITransposer transposer, object viewModel)
            {
                Transposer = transposer;
                ViewModel = viewModel;
            }

            public ITransposer Transposer { get; }
            public object ViewModel { get; }
        }

        #endregion
    }
}
