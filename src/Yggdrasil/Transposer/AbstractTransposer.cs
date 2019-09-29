namespace Yggdrasil.Transposer
{
    /// <summary>
    /// Abstract base implementation for transposer with access to view and viewmodel.
    /// </summary>
    /// <typeparam name="TView">Type of the view for the transposer.</typeparam>
    /// <typeparam name="TViewModel">Type of the viewmodel for the transposer.</typeparam>
    public abstract class AbstractTransposer<TView, TViewModel> : ITransposer
        where TView : class
        where TViewModel : class
    {
        #region Public Properties

        /// <summary>
        /// The view of the transposer.
        /// </summary>
        public TView View { get; private set; }

        /// <summary>
        /// The viewmodel of the transposer.
        /// </summary>
        public TViewModel ViewModel { get; private set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Specific implementation for tie.
        /// </summary>
        /// <param name="view">The tied view.</param>
        /// <param name="viewModel">The tied viewModel.f</param>
        protected abstract void Tie(TView view, TViewModel viewModel);

        /// <summary>
        /// Specific implementation for the untie.
        /// </summary>
        /// <param name="view">The tied view.</param>
        /// <param name="viewModel">The tied viewModel.</param>
        protected abstract void Untie(TView view, TViewModel viewModel);

        #endregion

        #region Interface Implementation

        #region ITransposer

        /// <see cref="ITransposer.OnTie(object, object)"/>
        public void OnTie(object view, object viewModel)
        {
            if (!(viewModel is TViewModel vModel) ||
                !(view is TView v))
                return;

            Tie(v, vModel);

            View = v;
            ViewModel = vModel;
        }

        /// <see cref="ITransposer.OnUntie(object, object)"/>
        public void OnUntie(object view, object viewModel)
        {
            if (!(viewModel is TViewModel vModel) ||
                !(view is TView v))
                return;

            Untie(v, vModel);

            View = null;
            ViewModel = null;
        }

        #endregion

        #endregion
    }
}
