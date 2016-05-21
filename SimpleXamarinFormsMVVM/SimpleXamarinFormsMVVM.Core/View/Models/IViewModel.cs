using System.Windows.Input;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View.Models
{
    public interface IViewModel
    {
        /// <summary>
        /// Sign that a new page should be put in the new <see cref="NavigationPage"/>.
        /// </summary>
        bool ShowInNewNavigationPage { get; }

        ICommand GoBackCommand { get; }
    }
}
