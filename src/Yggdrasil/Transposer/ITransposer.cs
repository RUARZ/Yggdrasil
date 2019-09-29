namespace Yggdrasil.Transposer
{
    /// <summary>
    /// Provides methods for tie and untie a transposer to view and viewmodels.
    /// </summary>
    public interface ITransposer
    {
        /// <summary>
        /// Method for tie the <paramref name="view"/> and <paramref name="viewModel"/>.
        /// </summary>
        /// <param name="view">The view to tie.</param>
        /// <param name="viewModel">The viewmodel to tie.</param>
        void OnTie(object view, object viewModel);
        /// <summary>
        /// Method for untieing the <paramref name="view"/> and <paramref name="viewModel"/>.
        /// </summary>
        /// <param name="view">The view to untie.</param>
        /// <param name="viewModel">The viewmodel to untie.</param>
        void OnUntie(object view, object viewModel);
    }
}
