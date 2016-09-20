using System;
using System.Collections.Generic;
using SimpleXamarinFormsMVVM.Core.View.Models;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View.Services
{
    public interface INavigationService
    {
        NavigationPage Root { get; }

        /// <summary>
        /// Stack of pages.
        /// </summary>
        Dictionary<IViewModel, Page> Stack { get; }

        /// <summary>
        /// Show page which mapped with <see cref="TViewModel"/>.
        /// </summary>
        /// <typeparam name="TViewModel">View model type.</typeparam>
        void ShowView<TViewModel>() where TViewModel : IViewModel;

        /// <summary>
        /// Show page which mapped with <see cref="TViewModel"/>.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="viewModelAdditionalAction">Additional action which executed after viewModel instance will be created (set route params for example).</param>
        void ShowView<TViewModel>(Action<TViewModel> viewModelAdditionalAction) where TViewModel : IViewModel;

        /// <summary>
        /// Show <see cref="MasterDetailPage"/>.
        /// Master page must be mapped with <see cref="TMasterViewModel"/>, detail page with <see cref="TDetailViewModel"/>.
        /// </summary>
        /// <typeparam name="TMasterViewModel"></typeparam>
        /// <typeparam name="TDetailViewModel"></typeparam>
        /// <param name="masterViewModelAdditionalAction"></param>
        /// <param name="detailViewModelAdditionalAction"></param>
        void ShowMasterView<TMasterViewModel, TDetailViewModel>(Action<TMasterViewModel> masterViewModelAdditionalAction = null, Action<TDetailViewModel> detailViewModelAdditionalAction = null)
            where TMasterViewModel : IMasterViewModel
            where TDetailViewModel : IDetailViewModel;

        void ChangeDetailView<TViewModel>(Action<TViewModel> viewModelAdditionalAction = null) where TViewModel : IDetailViewModel;

        /// <summary>
        /// Present master view (if contains in <see cref="Stack"/>).
        /// <see cref="MasterDetailPage.IsPresented"/>
        /// </summary>
        void PresentMasterView();

        /// <summary>
        /// Present detail view (if master contains in <see cref="Stack"/>).
        /// <see cref="MasterDetailPage.IsPresented"/>
        /// </summary>
        void PresentDetailView();

        /// <summary>
        /// Go to previous page in Stack.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Remove <see cref="viewModelToDelete"/> from stack (and all view models after this element in stack).
        /// </summary>
        /// <param name="viewModelToDelete"></param>
        void GoBack(IViewModel viewModelToDelete);

        /// <summary>
        /// Execute action for current page.
        /// </summary>
        /// <param name="action"></param>
        void Execute(Action<Page> action);
    }
}
