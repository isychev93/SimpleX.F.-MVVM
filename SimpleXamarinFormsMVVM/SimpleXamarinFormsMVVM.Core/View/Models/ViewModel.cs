using System.Windows.Input;
using SimpleXamarinFormsMVVM.Core.View.Services;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View.Models
{
    public abstract class ViewModel : BaseViewObject, IViewModel
    {
        protected readonly INavigationService navigationService;

        protected ViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            GoBackCommand = new Command(() => navigationService.GoBack());
        }

        public virtual bool ShowInNewNavigationPage
        {
            get { return false; }
        }

        public virtual void OnAppearing()
        {

        }

        public virtual void OnDisappearing()
        {

        }

        public virtual void InitModel()
        {

        }

        public virtual ICommand GoBackCommand { get; }
    }
}
