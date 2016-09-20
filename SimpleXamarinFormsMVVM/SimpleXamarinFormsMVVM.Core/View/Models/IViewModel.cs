using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View.Models
{
    public interface IViewModel : IDisposable
    {
        /// <summary>
        /// Sign that a new page should be put in the new <see cref="NavigationPage"/>.
        /// </summary>
        bool ShowInNewNavigationPage { get; }

        ICommand GoBackCommand { get; }

        /// <summary>
        /// Validate model before exit and call <see cref="SaveResult"/>.
        /// If function return false, exit will be canceled.
        /// </summary>
        /// <returns>
        /// true - model is valid, else false.
        /// </returns>
        bool ValidateBeforeExit();

        /// <summary>
        /// Save model result. Call only if <see cref="ValidateBeforeExit"/> return true.
        /// </summary>
        void SaveResult();

        void OnAppearing();
        void OnDisappearing();

        /// <summary>
        /// This function is called after performing additional operations, but before set model as DataContext.
        /// </summary>
        void InitModel();
    }
}
