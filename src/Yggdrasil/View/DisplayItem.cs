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

        private readonly RuleExecutor _ruleExecutor;
        private readonly CultureChangedSubscription _cultureChangedSubscription;

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
            _ruleExecutor = new RuleExecutor();
            _ruleExecutor.CreateLinks(view, viewModel);
            _ruleExecutor.SetResources(view);

            _cultureChangedSubscription = CultureManager.Subscribe(culture => _ruleExecutor.SetResources(View));
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

        #region Interface Implementation

        public void Dispose()
        {
            if (View is IDisposable disposableView)
                disposableView.Dispose();

            if (ViewModel is IDisposable disposableViewModel)
                disposableViewModel.Dispose();

            _cultureChangedSubscription?.Dispose();
            _ruleExecutor?.Dispose();
        }

        #endregion
    }
}
