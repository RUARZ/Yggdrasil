using System.Globalization;
using System.Threading;

namespace Yggdrasil.Resource
{
    public class ResourceManagerProvider : IResourceProvider
    {
        #region Private Fields

        private readonly System.Resources.ResourceManager _manager;

        #endregion

        #region Constructor

        public ResourceManagerProvider(System.Resources.ResourceManager manager)
        {
            _manager = manager;
        }

        #endregion

        #region Interface Implemention

        public string GetResource(string key)
        {
            return _manager.GetString(key, Thread.CurrentThread.CurrentUICulture);
        }

        public void LoadResources(CultureInfo culture)
        {
            // nothing to do?
        }

        #endregion
    }
}
