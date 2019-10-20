using System.Windows;

namespace Yggdrasil.Wpf.Helper
{
    class VisibilityHelper
    {
        #region Public Static Methods

        /// <summary>
        /// Returns a <see langword="bool"/> dependetn on <paramref name="visibility"/>. If <paramref name="visibility"/> is <see cref="Visibility.Visible"/> then
        /// <see langword="true"/> will be returned, otherwise <see langword="false"/>.
        /// </summary>
        /// <param name="visibility">The <see cref="Visibility"/> state to retrieve a <see langword="bool"/> from.</param>
        /// <returns>If <paramref name="visibility"/> is <see cref="Visibility.Visible"/> then <see langword="true"/> will be returned, otherwise <see langword="false"/>.</returns>
        public static bool VisibilityToBool(Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }

        /// <summary>
        /// Returns a <see cref="Visibility"/> state dependent on <paramref name="value"/>. If the <paramref name="value"/> is <see langword="true"/> then
        /// <see cref="Visibility.Visible"/> will be returned, otherwise <see cref="Visibility.Collapsed"/>.
        /// </summary>
        /// <param name="value">The value to convert to <see cref="Visibility"/>.</param>
        /// <returns>If the <paramref name="value"/> is <see langword="true"/> then <see cref="Visibility.Visible"/> will be returned, otherwise <see cref="Visibility.Collapsed"/>.</returns>
        public static Visibility BoolToVisibility(bool value)
        {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion
    }
}
