using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleXamarinFormsMVVM.Core.View.Models;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IPageLoaderService pageLoaderService;
        private readonly ITraceService traceService;
        private readonly Dictionary<IViewModel, Page> stack = new Dictionary<IViewModel, Page>();

        public NavigationService(IPageLoaderService pageLoaderService, ITraceService traceService)
        {
            this.pageLoaderService = pageLoaderService;
            this.traceService = traceService;

            Root = pageLoaderService.GetDefaultNavigationPage();
        }

        public Dictionary<IViewModel, Page> Stack
        {
            get { return stack; }
        }

        public NavigationPage Root { get; }

        public void ShowView<TViewModel>() where TViewModel : IViewModel
        {
            ShowView<TViewModel>(null);
        }

        public void ShowView<TViewModel>(Action<TViewModel> viewModelAdditionalAction) where TViewModel : IViewModel
        {
            if (typeof(TViewModel).GetTypeInfo().IsAssignableFrom(typeof(IDetailViewModel).GetTypeInfo()))
            {
                traceService.Trace("Can't show view {0}. View which implement IDetailViewModel must be present as detail of master view.", typeof(TViewModel));
                return;
            }

            var viewModelWithView = pageLoaderService.GetView(viewModelAdditionalAction);
            var model = viewModelWithView.Key;
            var view = viewModelWithView.Value;
            var currentNavigation = GetCurrentNavigation();

            PushViewInStack(model, view);

            if (!model.ShowInNewNavigationPage)
            {
                currentNavigation.PushAsync(view);
                return;
            }

            var newNavPage = pageLoaderService.GetDefaultNavigationPage();
            currentNavigation.PushAsync(newNavPage);
            newNavPage.PushAsync(view);
        }

        public void ShowMasterView<TMasterViewModel, TDetailViewModel>(Action<TMasterViewModel> masterViewModelAdditionalAction = null, Action<TDetailViewModel> detailViewModelAdditionalAction = null)
            where TMasterViewModel : IMasterViewModel
            where TDetailViewModel : IDetailViewModel
        {
            var masterViewModelWithView = pageLoaderService.GetView(masterViewModelAdditionalAction);
            var masterViewModel = masterViewModelWithView.Key;
            var masterView = masterViewModelWithView.Value;

            var detailViewModelWithView = pageLoaderService.GetView(detailViewModelAdditionalAction);
            var detailViewModel = detailViewModelWithView.Key;
            var detailView = detailViewModelWithView.Value;
            masterViewModel.DetailViewModel = detailViewModel;

            var masterPage = pageLoaderService.GetDefaultMasterPage();
            masterPage.Master = masterView;

            if (!detailViewModel.ShowInNewNavigationPage)
            {
                masterPage.Detail = detailView;
            }
            else
            {
                var newDetailNavPage = pageLoaderService.GetDefaultNavigationPage();
                newDetailNavPage.PushAsync(detailView);
                masterPage.Detail = newDetailNavPage;
            }

            var currentNavigation = GetCurrentNavigation();

            PushViewInStack(masterViewModel, masterView);
            PushViewInStack(detailViewModel, detailView);

            if (!masterViewModel.ShowInNewNavigationPage)
            {
                currentNavigation.PushAsync(masterPage);
            }
            else
            {
                var newMasterNavPage = pageLoaderService.GetDefaultNavigationPage();
                newMasterNavPage.PushAsync(masterPage);
            }
        }

        public void ChangeDetailView<TViewModel>(Action<TViewModel> viewModelAdditionalAction = null) where TViewModel : IDetailViewModel
        {
            var viewModelWithView = pageLoaderService.GetView(viewModelAdditionalAction);
            var model = viewModelWithView.Key;
            var view = viewModelWithView.Value;

            var masterInfo = GetCurrentMasterInfo();
            if (masterInfo == null)
            {
                traceService.Trace("Can't show {0} as detail view, masterInfo is null", typeof(TViewModel));
                return;
            }
            PopViewFromStack(masterInfo.DetailViewModelWithView.Key);
            PushViewInStack(model, view);

            if (!model.ShowInNewNavigationPage)
            {
                masterInfo.MasterDetailPage.Detail = view;
                return;
            }

            var newNavPage = pageLoaderService.GetDefaultNavigationPage();
            newNavPage.PushAsync(view);
            masterInfo.MasterDetailPage.Detail = newNavPage;
        }

        public void PresentMasterView()
        {
            var masterInfo = GetCurrentMasterInfo();
            if (masterInfo != null)
                masterInfo.MasterDetailPage.IsPresented = true;
        }

        public void PresentDetailView()
        {
            var masterInfo = GetCurrentMasterInfo();
            if (masterInfo != null)
                masterInfo.MasterDetailPage.IsPresented = false;
        }

        public void GoBack()
        {
            var currentPage = stack.Last();
            GoBack(currentPage.Key);
        }

        public void GoBack(IViewModel viewModelToDelete)
        {
            stack[viewModelToDelete].Navigation.PopAsync();
            PopViewFromStack(viewModelToDelete);
        }

        public void Execute(Action<Page> action)
        {
            action(stack.Last().Value);
        }

        private INavigation GetCurrentNavigation()
        {
            return Stack.Any() ? Stack.Last().Value.Navigation : Root.Navigation;
        }

        private MasterDetailPageInfo GetCurrentMasterInfo()
        {
            var masterViewModelWithView = stack.Last(p => p.Key is MasterViewModel);
            if (masterViewModelWithView.Key == null)
            {
                traceService.Trace("Can't get MasterInfo if no one MasterViewModel exists.");
                return null;
            }

            var detailViewModelWithView = stack.Last(p => p.Key is DetailViewModel);
            if (detailViewModelWithView.Key == null)
            {
                traceService.Trace("Can't get MasterInfo if no one DetailViewModel exists.");
                return null;
            }

            var masterDetailPage = masterViewModelWithView.Value.Parent as MasterDetailPage;
            if (masterDetailPage == null)
            {
                traceService.Trace("Can't MasterInfo if no one MasterDetailPage exists.");
                return null;
            }

            return new MasterDetailPageInfo(masterDetailPage,
                new KeyValuePair<IMasterViewModel, Page>(masterViewModelWithView.Key as IMasterViewModel, masterViewModelWithView.Value),
                new KeyValuePair<IDetailViewModel, Page>(detailViewModelWithView.Key as IDetailViewModel, detailViewModelWithView.Value));
        }

        private void PushViewInStack(IViewModel model, Page page)
        {
            stack.Add(model, page);
            SubscribeToPage(page);
        }

        private void PopViewFromStack(IViewModel model)
        {
            foreach (var pair in stack.SkipWhile(p => p.Key != model).Reverse().ToList())
            {
                stack.Remove(pair.Key);
                UnsubscribeFrom(pair.Value);
                model.Dispose();
            }
        }

        private void SubscribeToPage(Page newPage)
        {
            newPage.Appearing += Page_Appearing;
            newPage.Disappearing += Page_Disappearing;
        }

        private void UnsubscribeFrom(Page newPage)
        {
            newPage.Appearing -= Page_Appearing;
            newPage.Disappearing -= Page_Disappearing;
        }

        private void Page_Appearing(object sender, EventArgs eventArgs)
        {
            var viewModel = ((IViewModel)((Page)sender).BindingContext);
            viewModel.OnAppearing();
        }

        private void Page_Disappearing(object sender, EventArgs eventArgs)
        {
            var viewModel = ((IViewModel)((Page)sender).BindingContext);
            viewModel.OnDisappearing();
        }

        private sealed class MasterDetailPageInfo
        {
            public MasterDetailPageInfo(MasterDetailPage masterDetailPage, KeyValuePair<IMasterViewModel, Page> masterViewModelWithView, KeyValuePair<IDetailViewModel, Page> detailViewModelWithView)
            {
                MasterDetailPage = masterDetailPage;
                MasterViewModelWithView = masterViewModelWithView;
                DetailViewModelWithView = detailViewModelWithView;
            }

            public MasterDetailPage MasterDetailPage { get; }

            public KeyValuePair<IMasterViewModel, Page> MasterViewModelWithView { get; }

            public KeyValuePair<IDetailViewModel, Page> DetailViewModelWithView { get; }
        }
    }
}
