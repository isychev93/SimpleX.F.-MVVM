using SimpleXamarinFormsMVVM.Core.View.Models;
using SimpleXamarinFormsMVVM.Core.View.Services;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM
{
    public class TestMasterViewModel : MasterViewModel
    {
        public TestMasterViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            SelectPageCommand = new Command<string>(i =>
            {
                navigationService.ChangeDetailView<DetailViewModel>(d => d.Order = int.Parse(i));
            });
        }

        public Command<string> SelectPageCommand { get; }
    }
}
