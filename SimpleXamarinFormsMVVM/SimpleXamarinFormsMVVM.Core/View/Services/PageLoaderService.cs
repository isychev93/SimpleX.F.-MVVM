using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleXamarinFormsMVVM.Core.Extentions;
using SimpleXamarinFormsMVVM.Core.View.Models;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View.Services
{
    public class PageLoaderService : IPageLoaderService
    {
        private readonly Dictionary<Type, Type> viewModelViewMap = new Dictionary<Type, Type>();

        public void RegisterView<TViewModel, TView>() where TViewModel : IViewModel where TView : Page, IView<TViewModel>
        {
            viewModelViewMap.Add(typeof(TViewModel), typeof(TView));
        }

        public KeyValuePair<TViewModel, Page> GetView<TViewModel>() where TViewModel : IViewModel
        {
            return GetView<TViewModel>(null);
        }

        public KeyValuePair<TViewModel, Page> GetView<TViewModel>(Action<TViewModel> viewModelAdditionalAction) where TViewModel : IViewModel
        {

            var viewModelWithView = GetView(typeof(TViewModel), viewModel =>
            {
                if (viewModelAdditionalAction == null) return;
                viewModelAdditionalAction((TViewModel)viewModel);
            });

            return new KeyValuePair<TViewModel, Page>((TViewModel)viewModelWithView.Key, viewModelWithView.Value);
        }

        public KeyValuePair<IViewModel, Page> GetView(Type viewModelType, Action<IViewModel> viewModelAdditionalAction)
        {
            Type viewType;
            if (!viewModelViewMap.TryGetValue(viewModelType, out viewType))
                throw new InvalidOperationException(string.Format("Page with view model type {0} not registerd", viewModelType));

            var viewModel = CreateViewModel(viewModelType, viewModelAdditionalAction);
            var view = (Page)viewModelViewMap[viewModelType].CreateInstance();
            view.BindingContext = viewModel;
            var typeOfView = view.GetType();
            typeOfView.GetRuntimeProperty("Model").SetValue(view, viewModel);
            return new KeyValuePair<IViewModel, Page>(viewModel, view);
        }

        private static IViewModel CreateViewModel(Type viewModelType, Action<IViewModel> viewModelAdditionalAction)
        {
            var viewModel = (IViewModel)viewModelType.CreateInstance();
            viewModelAdditionalAction?.Invoke(viewModel);
            return viewModel;
        }

        public MasterDetailPage GetDefaultMasterPage<TMasterViewModel>() where TMasterViewModel : IMasterViewModel
        {
            return new MasterDetailPage
            {
                MasterBehavior = MasterBehavior.Popover,
                Title =  string.Empty
            };
        }
    }
}
