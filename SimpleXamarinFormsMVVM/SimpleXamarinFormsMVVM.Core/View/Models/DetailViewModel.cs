using System.Windows.Input;
using SimpleXamarinFormsMVVM.Core.View.Services;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View.Models
{
    public abstract class DetailViewModel : ViewModel, IDetailViewModel
    {
        protected DetailViewModel(INavigationService navigationService) : base(navigationService)
        {
            PresentMasterCommand = new Command(navigationService.PresentMasterView);
            PresentDetailCommand = new Command(navigationService.PresentDetailView);
        }

        public ICommand PresentMasterCommand { get; }
        public ICommand PresentDetailCommand { get; }
    }
}
