using System;
using System.Collections.Generic;
using SimpleXamarinFormsMVVM.Core.View.Models;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View.Services
{
    public interface IPageLoaderService
    {
        void RegisterView<TViewModel, TView>() where TViewModel : IViewModel where TView : Page, IView<TViewModel>;

        /// <summary>
        /// Create page which mapped with <see cref="TViewModel"/>.
        /// </summary>
        /// <remarks>
        /// To display the page use <see cref="INavigationService.ShowView{TViewModel}()"/> instead.
        /// </remarks>
        /// <typeparam name="TViewModel">View model type.</typeparam>
        KeyValuePair<TViewModel, Page> GetView<TViewModel>() where TViewModel : IViewModel;

        /// <summary>
        /// Create page which mapped with <see cref="TViewModel"/>.
        /// </summary>
        /// <remarks>
        /// To display the page use <see cref="INavigationService.ShowView{TViewModel}()"/> instead.
        /// </remarks>
        /// <typeparam name="TViewModel">View model type.</typeparam>
        /// <param name="viewModelAdditionalAction">Additional action which executed after viewModel instance will be created (set route params for example).</param>
        KeyValuePair<TViewModel, Page> GetView<TViewModel>(Action<TViewModel> viewModelAdditionalAction) where TViewModel : IViewModel;

        /// <summary>
        /// Create page which mapped with <see cref="viewModelType"/>.
        /// </summary>
        /// <remarks>
        /// To display the page use <see cref="INavigationService.ShowView{TViewModel}()"/> instead.
        /// </remarks>
        /// <param name="viewModelType">View model type.</param>
        /// <param name="viewModelAdditionalAction">Additional action which executed after viewModel instance will be created (set route params for example).</param>
        KeyValuePair<IViewModel, Page> GetView(Type viewModelType, Action<IViewModel> viewModelAdditionalAction);

        MasterDetailPage GetDefaultMasterPage();
        NavigationPage GetDefaultNavigationPage();
    }
}
