using SimpleXamarinFormsMVVM.Core.IOC;
using SimpleXamarinFormsMVVM.Core.View.Models;
using SimpleXamarinFormsMVVM.Core.View.Services;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core
{
    public abstract class MVVMApp : Application
    {
        private readonly MVVMSetup setup;

        protected MVVMApp(MVVMSetup setup)
        {
            this.setup = setup;

            setup.Initialize();
        }

        protected void ShowStartView<TViewModel>() where TViewModel : IViewModel
        {
            var navigationService = SimpleServiceLocator.Instance.Resolve<INavigationService>();
            navigationService.ShowView<TViewModel>();
            MainPage = navigationService.Root;
        }
    }
}
