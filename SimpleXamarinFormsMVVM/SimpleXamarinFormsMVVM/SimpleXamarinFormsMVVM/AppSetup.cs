using SimpleXamarinFormsMVVM.Core;
using SimpleXamarinFormsMVVM.Core.View.Services;

namespace SimpleXamarinFormsMVVM
{
    public class AppSetup : MVVMSetup
    {
        protected override void RegisterViews(IPageLoaderService pageLoaderService)
        {
            pageLoaderService.RegisterView<StartViewModel, StartView>();
            pageLoaderService.RegisterView<TestMasterViewModel, MasterPage>();
            pageLoaderService.RegisterView<DetailViewModel, DetailView>();
        }
    }
}
