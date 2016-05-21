using SimpleXamarinFormsMVVM.Core.View.Models;

namespace SimpleXamarinFormsMVVM.Core.View
{
    public interface IView<TViewModel> where TViewModel : IViewModel
    {
        TViewModel Model { get; set; }
    }
}
