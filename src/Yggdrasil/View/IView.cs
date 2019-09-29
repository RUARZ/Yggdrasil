namespace Yggdrasil
{
    /// <summary>
    /// Provides property for binding a view model and method to bury the view.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// The context of the view.
        /// </summary>
        object Context { get; set; }

        /// <summary>
        /// Displays the view.
        /// </summary>
        void Display();

        /// <summary>
        /// Bury the view.
        /// </summary>
        void Bury();
    }
}