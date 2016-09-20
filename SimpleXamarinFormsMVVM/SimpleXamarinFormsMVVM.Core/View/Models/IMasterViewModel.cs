using System.Windows.Input;

namespace SimpleXamarinFormsMVVM.Core.View.Models
{
    public interface IMasterViewModel : IViewModel
    {
        ICommand PresentMasterCommand { get; }
        ICommand PresentDetailCommand { get; }
    }
}
