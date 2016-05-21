using System.Windows.Input;
using SimpleXamarinFormsMVVM.Core.View.Services;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View.Models
{
    public abstract class MasterViewModel : BaseViewModel, IMasterViewModel
    {
        public MasterViewModel(INavigationService navigationService) : base(navigationService)
        {
            PresentMaster = new Command(navigationService.PresentMasterView);
        }

        public IDetailViewModel DetailViewModel { get; set; }

        public virtual ICommand PresentMaster { get; }
    }
}
