using SimpleXamarinFormsMVVM.Core.View.Models;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View
{
    public sealed class ViewModelWithView
    {
        public IViewModel ViewModel { get; }
        public Page View { get; }

        public ViewModelWithView(IViewModel viewModel, Page view)
        {
            ViewModel = viewModel;
            View = view;
        }
    }
}
