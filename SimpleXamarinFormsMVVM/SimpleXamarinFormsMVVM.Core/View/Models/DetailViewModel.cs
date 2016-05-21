using System.Windows.Input;
using SimpleXamarinFormsMVVM.Core.View.Services;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View.Models
{
    public abstract class DetailViewModel : BaseViewModel, IDetailViewModel
    {
        public DetailViewModel(INavigationService navigationService) : base(navigationService)
        {
            PresentDetail = new Command(navigationService.PresentDetailView);
        }

        public ICommand PresentDetail { get; }
    }
}
