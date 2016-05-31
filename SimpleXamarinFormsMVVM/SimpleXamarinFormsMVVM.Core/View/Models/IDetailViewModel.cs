using System.Windows.Input;

namespace SimpleXamarinFormsMVVM.Core.View.Models
{
    public interface IDetailViewModel : IViewModel
    {
        ICommand PresentMasterCommand { get; }
        ICommand PresentDetailCommand { get; }
    }
}
