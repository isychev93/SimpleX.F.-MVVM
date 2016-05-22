﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SimpleXamarinFormsMVVM.Core.View.Models;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View.Services
{
    public interface INavigationService
    {
        /// <summary>
        /// Stack of pages.
        /// Contain all types of pages, like <see cref="NavigationPage"/> and <see cref="MasterDetailPage"/>.
        /// </summary>
        /// <remarks>
        /// Exclusion: Not contains <see cref="MasterDetailPage.Master"/> in <see cref="MasterDetailPage"/>.
        /// </remarks>
        ReadOnlyObservableCollection<Page> Stack { get; }

        /// <summary>
        /// Show page which mapped with <see cref="TViewModel"/>.
        /// </summary>
        /// <typeparam name="TViewModel">View model type.</typeparam>
        /// <returns></returns>
        Task ShowView<TViewModel>() where TViewModel : IViewModel;

        /// <summary>
        /// Show page which mapped with <see cref="TViewModel"/>.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="viewModelAdditionalAction">Additional action which executed after viewModel instance will be created (set route params for example).</param>
        /// <returns></returns>
        Task ShowView<TViewModel>(Action<TViewModel> viewModelAdditionalAction) where TViewModel : IViewModel;

        /// <summary>
        /// Show <see cref="MasterDetailPage"/>.
        /// Master page must be mapped with <see cref="TMasterViewModel"/>, detail page with <see cref="TDetailViewModel"/>.
        /// </summary>
        /// <typeparam name="TMasterViewModel"></typeparam>
        /// <typeparam name="TDetailViewModel"></typeparam>
        /// <param name="masterViewModelAdditionalAction"></param>
        /// <param name="detailViewModelAdditionalAction"></param>
        /// <returns></returns>
        Task ShowMasterView<TMasterViewModel, TDetailViewModel>(Action<TMasterViewModel> masterViewModelAdditionalAction = null, Action<TDetailViewModel> detailViewModelAdditionalAction = null)
            where TMasterViewModel : IMasterViewModel
            where TDetailViewModel : IDetailViewModel;

        Task ChangeDetailView<TViewModel>(Action<TViewModel> viewModelAdditionalAction = null) where TViewModel : IViewModel;

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

        Task GoBack();

        /// <summary>
        /// Execute action for page which mapped with <see cref="viewModel"/>.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="action"></param>
        void Execute(IViewModel viewModel, Action<Page> action);
    }
}