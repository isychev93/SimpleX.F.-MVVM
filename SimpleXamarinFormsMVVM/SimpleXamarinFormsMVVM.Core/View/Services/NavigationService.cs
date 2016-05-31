using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SimpleXamarinFormsMVVM.Core.View.Models;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IPageLoaderService pageLoaderService;
        private readonly ITraceService traceService;
        private readonly Dictionary<IViewModel, Page> stack = new Dictionary<IViewModel, Page>();
        private readonly NavigationPage root = new NavigationPage();

        public NavigationService(IPageLoaderService pageLoaderService, ITraceService traceService)
        {
            this.pageLoaderService = pageLoaderService;
            this.traceService = traceService;
        }

        public Dictionary<IViewModel, Page> Stack
        {
            get { return stack; }
        }

        public NavigationPage Root
        {
            get { return root; }
        }

        public Task ShowView<TViewModel>() where TViewModel : IViewModel
        {
            return ShowView<TViewModel>(null);
        }

        public Task ShowView<TViewModel>(Action<TViewModel> viewModelAdditionalAction) where TViewModel : IViewModel
        {
            if (typeof(TViewModel).GetTypeInfo().IsAssignableFrom(typeof(IDetailViewModel).GetTypeInfo()))
            {
                traceService.Trace("Can't show view {0}. View which implement IDetailViewModel must be present as detail of master view.", typeof(TViewModel));
                return null;
            }

            var viewModelWithView = pageLoaderService.GetView(viewModelAdditionalAction);
            var model = viewModelWithView.Key;
            var view = viewModelWithView.Value;
            PushViewInStack(model, view);

            view.SetValue(NavigationPage.HasNavigationBarProperty, model.HasNavigationBar);

            var currentNavigation = GetCurrentNavigation();

            if (!model.ShowInNewNavigationPage)
            {
                var addNewViewTask = currentNavigation.PushAsync(view);
                return addNewViewTask;
            }

            var newNavPage = new NavigationPage();
            var addNewNavPageTask = currentNavigation.PushAsync(newNavPage);
            var addNewViewTaskIntoNewNavPage = newNavPage.PushAsync(view);
            return Task.WhenAll(addNewNavPageTask, addNewViewTaskIntoNewNavPage);
        }

        public Task ShowMasterView<TMasterViewModel, TDetailViewModel>(Action<TMasterViewModel> masterViewModelAdditionalAction = null, Action<TDetailViewModel> detailViewModelAdditionalAction = null)
            where TMasterViewModel : IMasterViewModel
            where TDetailViewModel : IDetailViewModel
        {
            var masterViewModelWithView = pageLoaderService.GetView(masterViewModelAdditionalAction);
            var masterViewModel = masterViewModelWithView.Key;
            var masterView = masterViewModelWithView.Value;
            masterViewModelWithView.Value.SetValue(NavigationPage.HasNavigationBarProperty, masterViewModel.HasNavigationBar);

            var detailViewModelWithView = pageLoaderService.GetView(detailViewModelAdditionalAction);
            var detailViewModel = detailViewModelWithView.Key;
            var detailView = detailViewModelWithView.Value;
            detailView.SetValue(NavigationPage.HasNavigationBarProperty, detailViewModel.HasNavigationBar);
            masterViewModel.DetailViewModel = detailViewModel;

            PushViewInStack(masterViewModel, masterView);
            PushViewInStack(detailViewModel, detailView);

            var tasksToWait = new List<Task>();
            var masterPage = pageLoaderService.GetDefaultMasterPage<TMasterViewModel>();
            masterPage.Master = masterView;

            if (!detailViewModel.ShowInNewNavigationPage)
            {
                masterPage.Detail = detailView;
            }
            else
            {
                var newDetailNavPage = new NavigationPage();
                tasksToWait.Add(newDetailNavPage.PushAsync(detailView));
            }

            var currentNavigation = GetCurrentNavigation();
            if (!masterViewModel.ShowInNewNavigationPage)
            {
                tasksToWait.Add(currentNavigation.PushAsync(masterPage));
            }
            else
            {
                var newMasterNavPage = new NavigationPage();
                tasksToWait.Add(newMasterNavPage.PushAsync(masterPage));
            }

            return Task.WhenAll(tasksToWait);
        }

        public Task ChangeDetailView<TViewModel>(Action<TViewModel> viewModelAdditionalAction = null) where TViewModel : IDetailViewModel
        {
            var viewModelWithView = pageLoaderService.GetView(viewModelAdditionalAction);
            var model = viewModelWithView.Key;
            var view = viewModelWithView.Value;

            var masterInfo = GetCurrentMasterInfo();
            if (masterInfo == null)
            {
                traceService.Trace("Can't show {0} as detail view, masterInfo is null", typeof(TViewModel));
                return null;
            }
            PopViewFromStack(masterInfo.DetailViewModelWithView.Key);
            PushViewInStack(model, view);

            if (!model.ShowInNewNavigationPage)
            {
                masterInfo.MasterDetailPage.Detail = view;
                return null;
            }

            var newNavPage = new NavigationPage();
            var addNewViewTaskIntoNewNavPage = newNavPage.PushAsync(view);
            masterInfo.MasterDetailPage.Detail = newNavPage;
            return addNewViewTaskIntoNewNavPage;
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

        public Task GoBack(IViewModel viewModelToDelete)
        {
            return stack[viewModelToDelete].Navigation.PopAsync();
        }

        public Task GoBack()
        {
            return stack.Last().Value.Navigation.PopAsync();
        }

        public void Execute(IViewModel viewModel, Action<Page> action)
        {
            action(stack[viewModel]);
        }

        private INavigation GetCurrentNavigation()
        {
            return Stack.Any() ? Stack.Last().Value.Navigation : root.Navigation;
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
                UnSubscribeFrom(pair.Value);
            }
        }

        private void SubscribeToPage(Page newPage)
        {
            newPage.Appearing += Page_Appearing;
            newPage.Disappearing += Page_Disappearing;
        }

        private void UnSubscribeFrom(Page newPage)
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
