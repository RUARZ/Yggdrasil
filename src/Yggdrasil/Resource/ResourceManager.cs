using Yggdrasil.Culture;
using Yggdrasil.Exceptions;

namespace Yggdrasil.Resource
{
    /// <summary>
    /// Provides methods for getting resource texts from a registered <see cref="IResourceProvider"/>.
    /// </summary>
    public static class ResourceHandler
    {
        #region Private Fields

        private static IResourceProvider _provider;

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers a implementation of <see cref="IResourceProvider"/>.
        /// </summary>
        /// <param name="provider">Implementation of <see cref="IResourceProvider"/> which should be registered.</param>
        /// <exception cref="ResourceProviderAlreadyRegisteredException">Thrown if a <see cref="IResourceProvider"/> was already registerd.</exception>
        public static void RegisterResourceProvider(IResourceProvider provider)
        {
            if (_provider != null)
                throw new ResourceProviderAlreadyRegisteredException("The resource provider was already registered!");

            _provider = provider;

            CultureManager.Subscribe(culture => _provider.LoadResources(culture));
        }

        /// <summary>
        /// Get's a resource for a passed <paramref name="resourceKey"/> from the <see cref="IResourceProvider"/>.
        /// </summary>
        /// <param name="resourceKey">The key of the resource which should  be retrieved.</param>
        /// <param name="keepEmptyString">If set to <see langword="true"/> then the method will return an empty string if no resource was found, otherwise
        /// it will return the <paramref name="resourceKey"/> surrounded with "???".</param>
        /// <returns>The resource text for the passed <paramref name="resourceKey"/>.</returns>
        /// <exception cref="ResourceProviderNotRegisteredException">Thrown if no <see cref="IResourceProvider"/> was registered.</exception>
        public static string GetResource(string resourceKey, bool keepEmptyString = false)
        {
            if (_provider == null)
                throw new ResourceProviderNotRegisteredException("No resource provider registered!");

            string resource = _provider.GetResource(resourceKey);

            if (string.IsNullOrEmpty(resource) && !keepEmptyString)
                return $"??? {resourceKey} ???";

            return _provider.GetResource(resourceKey) ?? string.Empty;
        }

        #endregion
    }
}
