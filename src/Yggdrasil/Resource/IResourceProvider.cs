using System.Globalization;

namespace Yggdrasil.Resource
{
    /// <summary>
    /// Provides methods for getting resource texts.
    /// </summary>
    public interface IResourceProvider
    {
        /// <summary>
        /// Get's the resource text for the passed <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key for retrieving the resource text.</param>
        /// <returns>The resource text for <paramref name="key"/>. If no text defined then <see langword="null"/> will be returned.</returns>
        string GetResource(string key);

        /// <summary>
        /// Loads resources for the passed <paramref name="culture"/>.
        /// </summary>
        /// <param name="culture">The culture for which resources should be loaded.</param>
        void LoadResources(CultureInfo culture);
    }
}
