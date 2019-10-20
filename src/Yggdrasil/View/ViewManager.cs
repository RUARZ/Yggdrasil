using System;
using System.Collections.Generic;
using System.Linq;
using Yggdrasil.Exceptions;
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
        
        private static IViewLocator _viewLocator;
        private static IViewHandler _viewHandler;
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
            _activeItems.Add(CreateAndDisplayItem(model));
        }

        /// <summary>
        /// Displays the model with the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the model which should be displayed.</typeparam>
        /// <exception cref="Exception"></exception>
        public static void DisplayModel<T>() where T : new()
        {
            DisplayModel(Activator.CreateInstance(typeof(T)));
        }

        /// <summary>
        /// Displays the model with the passed <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the model which should be displayed.</param>
        /// <exception cref="Exception"></exception>
        public static void DisplayModel(Type type)
        {
            DisplayModel(Activator.CreateInstance(type));
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
        /// Displays the model with the passed <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the model which should be displayed.</param>
        /// <param name="constructorParameters">Parameters for creating a instance of the type <typeparamref name="T"/>.</param>
        /// <exception cref="Exception"></exception>
        public static void DisplayModel(Type type, params object[] constructorParameters)
        {
            DisplayModel(Activator.CreateInstance(type, constructorParameters));
        }

        /// <summary>
        /// Displays the passed <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The model which should be displayed.</param>
        /// <param name="owner">The owner view model for the display modal.</param>
        /// <exception cref="Exception"></exception>
        public static void DisplayModalModel(object owner, object model)
        {
            CreateAndDisplayModalItem(owner, model);
        }
        
        /// <summary>
        /// Displays the model with the type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="owner">The owner view model for the display modal.</param>
        /// <typeparam name="T">The type of the model which should be displayed.</typeparam>
        /// <exception cref="Exception"></exception>
        public static void DisplayModalModel<T>(object owner) where T : new()
        {
            DisplayModalModel(owner, Activator.CreateInstance(typeof(T)));
        }

        /// <summary>
        /// Displays the model with the passed <paramref name="type"/>.
        /// </summary>
        /// <param name="owner">The owner view model for the display modal.</param>
        /// <param name="type">The <see cref="Type"/> of the model which should be displayed.</param>
        /// <exception cref="Exception"></exception>
        public static void DisplayModalModel(object owner, Type type)
        {
            DisplayModalModel(owner, Activator.CreateInstance(type));
        }

        /// <summary>
        /// Displays the model with the type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="owner">The owner view model for the display modal.</param>
        /// <typeparam name="T">The type of the model which should be displayed.</typeparam>
        /// <param name="constructorParameters">Parameters for creating a instance of the type <typeparamref name="T"/>.</param>
        /// <exception cref="Exception"></exception>
        public static void DisplayModalModel<T>(object owner, params object[] constructorParameters)
        {
            DisplayModalModel(owner, Activator.CreateInstance(typeof(T), constructorParameters));
        }

        /// <summary>
        /// Displays the model with the passed <paramref name="type"/>.
        /// </summary>
        /// <param name="owner">The owner view model for the display modal.</param>
        /// <param name="type">The <see cref="Type"/> of the model which should be displayed.</param>
        /// <param name="constructorParameters">Parameters for creating a instance of the type <typeparamref name="T"/>.</param>
        /// <exception cref="Exception"></exception>
        public static void DisplayModalModel(object owner, Type type, params object[] constructorParameters)
        {
            DisplayModalModel(owner, Activator.CreateInstance(type, constructorParameters));
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

            _rootItem = CreateAndDisplayItem(model);
        }

        /// <summary>
        /// Displays the model with the type <typeparamref name="T"/> and set it as root model.
        /// </summary>
        /// <typeparam name="T">The type of the model which should be displayed.</typeparam>
        /// <exception cref="RootModelAlreadyDisplayedException">Thrown if there is already a registered root model.</exception>
        /// <exception cref="RootModelNoLongerAllowedException">Thrown if there are already other models displayed and therefore no rootmodel can be set anymore.</exception>
        /// <exception cref="Exception"></exception>
        public static void DisplayRootModel<T>() where T : new()
        {
            DisplayRootModel(Activator.CreateInstance(typeof(T)));
        }

        /// <summary>
        /// Displays the model with the passed <paramref name="type"/> and set it as root model.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the model which should be displayed.</param>
        /// <exception cref="Exception"></exception>
        public static void DisplayRootModel(Type type)
        {
            DisplayRootModel(Activator.CreateInstance(type));
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
        /// Displays the model with the passed <paramref name="type"/> and set it as root model.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the model which should be displayed.</param>
        /// <param name="constructorParameters">Parameters for creating a instance of the type <typeparamref name="T"/>.</param>
        /// <exception cref="Exception"></exception>
        public static void DisplayRootModel(Type type, params object[] constructorParameters)
        {
            DisplayRootModel(Activator.CreateInstance(type, constructorParameters));
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
        /// <exception cref="AlreadyRegisteredException">Thrown if there is already a implementation of <see cref="IViewLocator"/> registerd.</exception>
        public static void RegisterViewLocator(IViewLocator viewLocator)
        {
            if (_viewLocator != null)
                throw new AlreadyRegisteredException($"A implementation of '{nameof(IViewLocator)}' is already registered!");

            _viewLocator = viewLocator;
        }

        /// <summary>
        /// Registers a implemenation of <see cref="IViewHandler"/> for handling view actions.
        /// </summary>
        /// <param name="viewHandler">The implememntation of <see cref="IViewHandler"/> which should be registered.</param>
        /// <exception cref="AlreadyRegisteredException">Thrown if there is already a implementation of <see cref="IViewHandler"/> registered.</exception>
        public static void RegisterViewHandler(IViewHandler viewHandler)
        {
            if (_viewHandler != null)
                throw new AlreadyRegisteredException($"A implementation of '{nameof(IViewHandler)}' is already registered!");

            _viewHandler = viewHandler;
            _viewHandler.RegisterGetViewForViewModelFunction(GetViewForViewModel);
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
        /// Get's the context of the passed <paramref name="view"/>.
        /// </summary>
        /// <param name="view">The view for which the context should be retrieved.</param>
        /// <returns>The context of the passed <paramref name="view"/>.</returns>
        internal static object GetContextOfView(object view)
        {
            CheckViewHandlerRegistration();

            return _viewHandler.GetContext(view);
        }

        #endregion

        #region Private Methods

        private static DisplayItem CreateAndDisplayItem(object model)
        {
            DisplayItem item = CreateItem(model);

            _viewHandler.Display(item.View);

            return item;
        }

        private static void CreateAndDisplayModalItem(object owner, object model)
        {
            DisplayItem item = CreateItem(model);
            //the item must be added to the active items now because the modal display
            //could block the further execution until the dialog is closed and therefore the bury method will throw a exception
            _activeItems.Add(item);

            _viewHandler.DisplayModal(owner, item.View);
        }

        private static DisplayItem CreateItem(object model)
        {
            CheckViewHandlerRegistration();

            object view = CreateView(model);

            _viewHandler.SetContext(view, model);

            DisplayItem item = new DisplayItem(view, model);

            TransposerManager.Enable(view);

            if (model is AbstractBaseViewModel baseModel)
                baseModel.Initialize();

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
            _viewHandler.Bury(item.View);

            TransposerManager.Disable(item.View);

            item.Dispose();
        }

        private static void CheckViewHandlerRegistration()
        {
            if (_viewHandler == null)
                throw new NotRegisteredException($"A instance of '{typeof(IViewHandler).Name}' was not registered! " +
                    $"Register a implementation of '{typeof(IViewHandler).Name}' with '{nameof(RegisterViewHandler)}'!");
        }

        #endregion
    }
}