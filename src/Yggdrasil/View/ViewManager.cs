using System;
using System.Collections.Generic;
using System.Linq;
using Yggdrasil.Transposer;
using Yggdrasil.ViewModel;

namespace Yggdrasil
{
    /// <summary>
    /// Handles the displaying and burying of models with their views.
    /// </summary>
    public static class ViewManager
    {
        #region Private Fields

        private static readonly List<ViewTypeInfo> _viewTypeInfos = new List<ViewTypeInfo>();
        private static IViewLocator _viewLocator;
        private static readonly List<DisplayItem> _activeItems = new List<DisplayItem>();
        private static DisplayItem _rootItem;

        #endregion

        #region Public Methods

        /// <summary>
        /// Displays the passed <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The model which should be displayed.</param>
        /// <exception cref="Exception"></exception>
        public static void DisplayModel(object model)
        {
            _activeItems.Add(CreateItem(model));
        }

        /// <summary>
        /// Displays the model with the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the model which should be displayed.</typeparam>
        /// <exception cref="Exception"></exception>
        public static void DisplayModel<T>()
        {
            DisplayModel(Activator.CreateInstance(typeof(T)));
        }

        /// <summary>
        /// Displays the model with the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the model which should be displayed.</typeparam>
        /// <param name="constructorParameters">Parameters for creating a instance of the type <typeparamref name="T"/>.</param>
        /// <exception cref="Exception"></exception>
        public static void DisplayModel<T>(params object[] constructorParameters)
        {
            DisplayModel(Activator.CreateInstance(typeof(T), constructorParameters));
        }

        /// <summary>
        /// Displays the passed <paramref name="model"/> and set it as root model.
        /// </summary>
        /// <param name="model">The model which should be displayed.</param>
        /// <exception cref="RootModelAlreadyDisplayedException">Thrown if there is already a registered root model.</exception>
        /// <exception cref="RootModelNoLongerAllowedException">Thrown if there are already other models displayed and therefore no rootmodel can be set anymore.</exception>
        /// <exception cref="Exception"></exception>
        public static void DisplayRootModel(object model)
        {
            if (_rootItem != null)
                throw new RootModelAlreadyDisplayedException("There is already a root model displayed!");

            if (_activeItems.Count > 0)
                throw new RootModelNoLongerAllowedException("Displaying of a root model is no longer allowed because there are already other models displayed!");

            _rootItem = CreateItem(model);
        }

        /// <summary>
        /// Displays the model with the type <typeparamref name="T"/> and set it as root model.
        /// </summary>
        /// <typeparam name="T">The type of the model which should be displayed.</typeparam>
        /// <exception cref="RootModelAlreadyDisplayedException">Thrown if there is already a registered root model.</exception>
        /// <exception cref="RootModelNoLongerAllowedException">Thrown if there are already other models displayed and therefore no rootmodel can be set anymore.</exception>
        /// <exception cref="Exception"></exception>
        public static void DisplayRootModel<T>()
        {
            DisplayRootModel(Activator.CreateInstance(typeof(T)));
        }

        /// <summary>
        /// Displays the model with the type <typeparamref name="T"/> and set it as root model.
        /// </summary>
        /// <typeparam name="T">The type of the model which should be displayed.</typeparam>
        /// <param name="constructorParameters">Parameters for creating a instance of the type <typeparamref name="T"/>.</param>
        /// <exception cref="RootModelAlreadyDisplayedException">Thrown if there is already a registered root model.</exception>
        /// <exception cref="RootModelNoLongerAllowedException">Thrown if there are already other models displayed and therefore no rootmodel can be set anymore.</exception>
        /// <exception cref="Exception"></exception>
        public static void DisplayRootModel<T>(params object[] constructorParameters)
        {
            DisplayRootModel(Activator.CreateInstance(typeof(T), constructorParameters));
        }

        /// <summary>
        /// Bury the passed <paramref name="model"/> and calls Dispose method if the <see cref="IDisposable"/> interface is implemented. 
        /// </summary>
        /// <param name="model">The model which should be buried</param>
        /// <exception cref="ModelNotFoundException">Thrown if the passed <paramref name="model"/> can't be found in the active models.</exception>
        public static void BuryModel(object model)
        {
            if (_rootItem != null && ReferenceEquals(model, _rootItem.ViewModel))
            {
                BuryItem(_rootItem);

                for (int i = _activeItems.Count - 1; i >= 0; i--)
                {
                    BuryItem(_activeItems[i]);
                }
            }
            else
            {
                DisplayItem item = _activeItems.FirstOrDefault(x => ReferenceEquals(x.ViewModel, model));

                if (item.View == null || item.ViewModel == null)
                    throw new ModelNotFoundException("The passed model to bury was not found!");

                BuryItem(item);
                _activeItems.Remove(item);
            }
        }


        /// <summary>
        /// Registers a implementation of <see cref="IViewLocator"/> for locating views.
        /// </summary>
        /// <param name="viewLocator">The implementation of <see cref="IViewLocator"/> which should be registered.</param>
        /// <exception cref="ViewLocatorAlreadyRegisteredException">Thrown if there is already a implementation of <see cref="IViewLocator"/> registerd.</exception>
        public static void RegisterViewLocator(IViewLocator viewLocator)
        {
            if (_viewLocator != null)
                throw new ViewLocatorAlreadyRegisteredException($"A implementation of '{nameof(IViewLocator)}' is already registered!");

            _viewLocator = viewLocator;
        }

        /// <summary>
        /// Adds informations about view types about the context property name and the method for burying views.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which the definitions are valid.</param>
        /// <param name="contextPropertyName">The name of the context property to set the view model.</param>
        /// <param name="displayMethodName">The name of the method to display the view.</param>
        /// <param name="buryMethodName">The name of the method which should be executed to bury a view.</param>
        /// <param name="buriedEventName">The name of the event after the view was buried.</param>
        public static void AddViewTypeInfo(Type type, string contextPropertyName, string displayMethodName, string buryMethodName, string buriedEventName)
        {
            _viewTypeInfos.Add(new ViewTypeInfo(type, contextPropertyName, displayMethodName, buryMethodName, buriedEventName));
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Searches for the belonging view to the passed <paramref name="viewModel"/>.
        /// </summary>
        /// <param name="viewModel">The view model instance for searching the belonging view.</param>
        /// <returns>The view for the <paramref name="viewModel"/>.</returns>
        internal static object GetViewForViewModel(object viewModel)
        {
            if (_rootItem != null && ReferenceEquals(_rootItem.ViewModel, viewModel))
                return _rootItem.View;

            DisplayItem item = _activeItems.Find(x => ReferenceEquals(x.ViewModel, viewModel));

            return item?.View;
        }

        /// <summary>
        /// Get's a <see cref="ViewTypeInfo"/> for the passed <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the view for retrieving the info.</param>
        /// <returns><see cref="ViewTypeInfo"/> for the passed <paramref name="type"/>.</returns>
        /// <exception cref="Exception"></exception>
        internal static ViewTypeInfo GetViewTypeInfo(Type type)
        {
            foreach (ViewTypeInfo info in _viewTypeInfos)
            {
                if (info.Type.IsAssignableFrom(type))
                    return info;
            }

            throw new KeyNotFoundException($"For the type '{type.GetType()}' where no infos set!");
        }

        #endregion

        #region Private Methods

        private static DisplayItem CreateItem(object model)
        {
            object view = CreateView(model);

            IView v = view as IView;

            if (v != null)
                v.Context = model;
            else
                GetViewTypeInfo(view.GetType()).ContextPropertyInfo.SetValue(view, model);

            DisplayItem item = new DisplayItem(view, model);

            TransposerManager.Enable(view);

            if (model is AbstractBaseViewModel baseModel)
                baseModel.Initialize();

            if (v != null)
                v.Display();
            else
                GetViewTypeInfo(view.GetType()).DisplayMethodInfo.Invoke(view, null);

            return item;
        }

        private static object CreateView(object model)
        {
            if (_viewLocator != null)
                return _viewLocator.GetView(model);

            Type viewType = RuleProvider.GetViewType(model.GetType());

            return Activator.CreateInstance(viewType);
        }

        private static void BuryItem(DisplayItem item)
        {
            if (item.View is IView v) 
                v.Bury();
            else
                GetViewTypeInfo(item.View.GetType()).BuryMethodInfo.Invoke(item.View, null);

            TransposerManager.Disable(item.View);

            item.Dispose();
        }

        #endregion
    }
}