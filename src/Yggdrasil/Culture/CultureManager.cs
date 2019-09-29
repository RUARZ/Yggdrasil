using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Yggdrasil.Culture
{
    /// <summary>
    /// Manager for handling culture changes and getting culture resources.
    /// </summary>
    public static class CultureManager
    {
        #region Private Fields

        private static readonly List<WeakReference> _subscriptions = new List<WeakReference>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Subscribes a culture changed action and returns the created <see cref="CultureChangedSubscription"/>.
        /// </summary>
        /// <param name="cultureChangedAction">The action which should be executed on a culture change.</param>
        /// <returns>The new created <see cref="CultureChangedSubscription"/>.</returns>
        public static CultureChangedSubscription Subscribe(Action<CultureInfo> cultureChangedAction)
        {
            CultureChangedSubscription subscription = new CultureChangedSubscription(cultureChangedAction,
                s =>
                {
                    WeakReference sub = _subscriptions.FirstOrDefault(x => x.Target == s);

                    if (sub != null)
                        _subscriptions.Remove(sub);
                });

            _subscriptions.Add(new WeakReference(subscription));

            return subscription;
        }

        /// <summary>
        /// Changes the culture of the current <see cref="Thread"/> to the culture with the passed <paramref name="cultureName"/> and executes all alive
        /// subscriptions.
        /// </summary>
        /// <param name="cultureName">The name of the culture which should be set.</param>
        public static void ChangeCulture(string cultureName)
        {
            CultureInfo culture = new CultureInfo(cultureName);

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            for (int i = _subscriptions.Count - 1; i >= 0; i--)
            {
                if (!_subscriptions[i].IsAlive)
                {
                    _subscriptions.RemoveAt(i);
                    continue;
                }

                ((CultureChangedSubscription)_subscriptions[i].Target).ExecuteCultureChanged(culture);
            }
        }

        #endregion
    }
}
