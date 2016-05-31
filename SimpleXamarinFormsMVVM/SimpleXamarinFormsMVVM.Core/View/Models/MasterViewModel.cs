using System.Windows.Input;
using SimpleXamarinFormsMVVM.Core.View.Services;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View.Models
{
    public abstract class MasterViewModel : ViewModel, IMasterViewModel
    {
        protected MasterViewModel(INavigationService navigationService) : base(navigationService)
        {
            PresentMasterCommand = new Command(navigationService.PresentMasterView);
            PresentDetailCommand = new Command(navigationService.PresentDetailView);
            GoBackCommand = new Command(() => navigationService.GoBack(this));
        }

        public IDetailViewModel DetailViewModel { get; set; }

        public virtual ICommand PresentMasterCommand { get; }
        public virtual ICommand PresentDetailCommand { get; }

        public override ICommand GoBackCommand { get; }
    }
}
