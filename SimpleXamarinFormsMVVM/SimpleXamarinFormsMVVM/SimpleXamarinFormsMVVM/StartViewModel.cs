using System.Windows.Input;
using SimpleXamarinFormsMVVM.Core.View.Models;
using SimpleXamarinFormsMVVM.Core.View.Services;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM
{
    public class StartViewModel : BaseViewModel
    {
        public StartViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public string MainText
        {
            get
            {
                return "HELLO WORLD";
            }
        }

        public ICommand NextPageCommand
        {
            get { return new Command(() => navigationService.ShowView<StartViewModel>()); }
        }

        public ICommand MasterPageCommand
        {
            get { return new Command(() => navigationService.ShowMasterView<TestMasterViewModel, DetailViewModel>(null, d => d.Order = 1)); }
        }
    }
}
