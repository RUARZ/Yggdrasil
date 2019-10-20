using System;

namespace Yggdrasil
{
    /// <summary>
    /// Handles operations with a view e.g. setting datacontext, displaying/bury view, ...
    /// </summary>
    public interface IViewHandler
    {
        /// <summary>
        /// Set's the <paramref name="viewModel"/> as context of the <paramref name="view"/>.
        /// </summary>
        /// <param name="view">The view for which the context should be set.</param>
        /// <param name="viewModel">The viewModel which should be set as context of the <paramref name="view"/>.</param>
        void SetContext(object view, object viewModel);
        
        /// <summary>
        /// Get's the currently set context from the passed <paramref name="view"/>.
        /// </summary>
        /// <param name="view">The view for which the context should be retrieved.</param>
        /// <returns>The context of the passed <paramref name="view"/>.</returns>
        object GetContext(object view);

        /// <summary>
        /// Displays the passed <paramref name="view"/>.
        /// </summary>
        /// <param name="view">The view which should be displayed.</param>
        void Display(object view);

        /// <summary>
        /// Displays the passed <paramref name="view"/> modal.
        /// </summary>
        /// <param name="owner">The owner for displaying modal.</param>
        /// <param name="view">The view which should be displayed modal.</param>
        void DisplayModal(object owner, object view);

        /// <summary>
        /// Buries the passed <paramref name="view"/>.
        /// </summary>
        /// <param name="view">The view which should be buried.</param>
        void Bury(object view);

        /// <summary>
        /// Passes a <see cref="Func{T, TResult}"/> for getting the view of a viewmodel.
        /// </summary>
        /// <param name="getViewForViewModelFunc">The <see cref="Func{T, TResult}"/> for getting the view of a view model.</param>
        void RegisterGetViewForViewModelFunction(Func<object, object> getViewForViewModelFunc);
    }

}
