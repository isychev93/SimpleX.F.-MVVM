using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using SimpleXamarinFormsMVVM.Core.Extentions;
using SimpleXamarinFormsMVVM.Core.View.Models;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IPageLoaderService pageLoaderService;
        private readonly ITraceService traceService;
        private readonly ObservableCollection<Page> stack = new ObservableCollection<Page>();

        public NavigationService(IPageLoaderService pageLoaderService, ITraceService traceService)
        {
            this.pageLoaderService = pageLoaderService;
            this.traceService = traceService;

            Stack = new ReadOnlyObservableCollection<Page>(stack);
            stack.Add(new NavigationPage());
            stack.CollectionChanged += Stack_CollectionChanged;
        }

        public ReadOnlyObservableCollection<Page> Stack { get; }

        public Task ShowView<TViewModel>() where TViewModel : IViewModel
        {
            return ShowView<TViewModel>(null);
        }

        public Task ShowView<TViewModel>(Action<TViewModel> viewModelAdditionalAction) where TViewModel : IViewModel
        {
            var viewModelWithView = pageLoaderService.GetView(viewModelAdditionalAction);
            var model = viewModelWithView.Key;
            var view = viewModelWithView.Value;
            view.SetValue(NavigationPage.HasNavigationBarProperty, model.HasNavigationBar);

            var lastNavPage = GetLastNavigationPage();
            if (lastNavPage == null)
                throw new InvalidOperationException("Can't show page if no one NavigationPage exists.");

            if (!model.ShowInNewNavigationPage)
            {
                var addNewViewTask = lastNavPage.PushAsync(view);
                stack.Add(view);
                return addNewViewTask;
            }

            var newNavPage = new NavigationPage();
            var addNewNavPageTask = lastNavPage.PushAsync(newNavPage);
            var addNewViewTaskIntoNewNavPage = newNavPage.PushAsync(view);
            stack.Add(newNavPage);
            stack.Add(view);
            return Task.WhenAll(addNewNavPageTask, addNewViewTaskIntoNewNavPage);
        }

        public Task ShowMasterView<TMasterViewModel, TDetailViewModel>(Action<TMasterViewModel> masterViewModelAdditionalAction = null, Action<TDetailViewModel> detailViewModelAdditionalAction = null)
            where TMasterViewModel : IMasterViewModel
            where TDetailViewModel : IDetailViewModel
        {
            var masterViewModelWithView = pageLoaderService.GetView(masterViewModelAdditionalAction);
            var masterViewModel = masterViewModelWithView.Key;
            masterViewModelWithView.Value.SetValue(NavigationPage.HasNavigationBarProperty, masterViewModel.HasNavigationBar);

            var detailViewModelWithView = pageLoaderService.GetView(detailViewModelAdditionalAction);
            var detailViewModel = detailViewModelWithView.Key;
            var detailView = detailViewModelWithView.Value;
            detailView.SetValue(NavigationPage.HasNavigationBarProperty, detailViewModel.HasNavigationBar);
            masterViewModel.DetailViewModel = detailViewModel;

            var pagesToAdd = new List<Page>();
            var tasksToWait = new List<Task>();
            var masterPage = new MasterDetailPage { Master = masterViewModelWithView.Value, Title = string.Empty };
            if (!detailViewModel.ShowInNewNavigationPage)
            {
                masterPage.Detail = detailView;
                pagesToAdd.Add(detailView);
            }
            else
            {
                var newDetailNavPage = new NavigationPage();
                tasksToWait.Add(newDetailNavPage.PushAsync(detailView));
                pagesToAdd.Add(newDetailNavPage);
                pagesToAdd.Add(detailView);
            }

            var lastNavPage = GetLastNavigationPage();
            if (!masterViewModel.ShowInNewNavigationPage)
            {
                tasksToWait.Add(lastNavPage.PushAsync(masterPage));
                pagesToAdd.Insert(0, masterPage);
            }
            else
            {
                var newMasterNavPage = new NavigationPage();
                tasksToWait.Add(newMasterNavPage.PushAsync(masterPage));
                pagesToAdd.Insert(0, newMasterNavPage);
                pagesToAdd.Insert(1, masterPage);
            }

            stack.AddRange(pagesToAdd);
            return Task.WhenAll(tasksToWait);
        }

        public Task ChangeDetailView<TViewModel>(Action<TViewModel> viewModelAdditionalAction = null) where TViewModel : IViewModel
        {
            var viewModelWithView = pageLoaderService.GetView(viewModelAdditionalAction);
            var model = viewModelWithView.Key;
            var view = viewModelWithView.Value;
            var master = stack.OfType<MasterDetailPage>().Last();
            if (master == null)
            {
                traceService.Trace("Can't show detail page if no one master page exists.");
                return null;
            }

            if (!model.ShowInNewNavigationPage)
            {
                master.Detail = view;
                stack.Add(view);
                return null;
            }

            var newNavPage = new NavigationPage();
            var addNewViewTaskIntoNewNavPage = newNavPage.PushAsync(view);
            master.Detail = newNavPage;
            stack.Add(newNavPage);
            stack.Add(view);
            return addNewViewTaskIntoNewNavPage;
        }

        public void PresentMasterView()
        {
            stack.OfType<MasterDetailPage>().Last().IsPresented = true;
        }

        public void PresentDetailView()
        {
            stack.OfType<MasterDetailPage>().Last().IsPresented = false;
        }

        public Task GoBack()
        {
            return GoBack(null);
        }

        private Task GoBack(Task previousPopTask, bool continueOnlyIfCurrentPageIsNavigationPage = false)
        {
            if (stack.Count <= 2)
            {
                traceService.Trace("Can't go back, current page is root.");
                return null;
            }

            var currentPage = stack.Last();

            if (continueOnlyIfCurrentPageIsNavigationPage && !(currentPage is NavigationPage))
                return previousPopTask;

            var tasksToWait = new List<Task>();
            if (previousPopTask != null)
                tasksToWait.Add(previousPopTask);
            var navigationPages = stack.OfType<NavigationPage>().ToList();
            var beforeCurrentPage = BeforePage(currentPage);
            // currentPage = currentNavPage
            if (currentPage is NavigationPage)
            {
                var navPageOfCurrentNavPage = navigationPages[navigationPages.Count - 2];
                // NavPage in MasterDetailPage.Detail
                if (beforeCurrentPage is MasterDetailPage)
                {
                    stack.Remove(currentPage);
                    // Remove MasterDetailPage from stack
                    tasksToWait.Add(navPageOfCurrentNavPage.PopAsync());
                    stack.Remove(beforeCurrentPage);
                    return GoBack(Task.WhenAll(tasksToWait), true);
                }

                stack.Remove(currentPage);
                tasksToWait.Add(navPageOfCurrentNavPage.PopAsync());
                return GoBack(Task.WhenAll(tasksToWait), true);
            }

            var currentNavPage = navigationPages.Last();
            stack.Remove(currentPage);
            tasksToWait.Add(currentNavPage.PopAsync());
            // We must delete MasterDetailPage too.
            if (beforeCurrentPage is MasterDetailPage)
                return GoBack(Task.WhenAll(tasksToWait));

            return GoBack(Task.WhenAll(tasksToWait), true);
        }

        public void Execute(IViewModel viewModel, Action<Page> action)
        {
            action(stack.Single(p => p.BindingContext == viewModel));
        }

        private NavigationPage GetLastNavigationPage()
        {
            return stack.OfType<NavigationPage>().Last();
        }

        private Page BeforePage(Page page)
        {
            return stack[stack.IndexOf(page) - 1];
        }

        private void Stack_CollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var newPage in notifyCollectionChangedEventArgs.NewItems.OfType<Page>())
                            SubscribeToPageIfNeeded(newPage);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var oldPage in notifyCollectionChangedEventArgs.OldItems.OfType<Page>())
                            UnSubscribeFromPageIfNeeded(oldPage);
                    }
                    break;
            }
        }

        private void SubscribeToPageIfNeeded(Page newPage)
        {
            var viewModel = newPage.BindingContext as IViewModel;
            if (viewModel != null)
            {
                newPage.Appearing += Page_Appearing;
                newPage.Disappearing += Page_Disappearing;
            }
        }

        private void UnSubscribeFromPageIfNeeded(Page newPage)
        {
            var viewModel = newPage.BindingContext as IViewModel;
            if (viewModel != null)
            {
                newPage.Appearing -= Page_Appearing;
                newPage.Disappearing -= Page_Disappearing;
            }
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
    }
}
