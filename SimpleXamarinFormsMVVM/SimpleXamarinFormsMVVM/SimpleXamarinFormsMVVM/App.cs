using SimpleXamarinFormsMVVM.Core;

namespace SimpleXamarinFormsMVVM
{
    public class App : MVVMApp
    {
        public App() : base(new AppSetup())
        {
            ShowStartViewModel<StartViewModel>();
        }
    }
}
