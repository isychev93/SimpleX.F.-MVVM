using System.Windows.Input;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View.Models
{
    public interface IMasterViewModel : IViewModel
    {
        /// <summary>
        /// Current <see cref="MasterDetailPage.Detail"/> page.
        /// </summary>
        IDetailViewModel DetailViewModel { get; set; }

        ICommand PresentMaster { get; }
    }
}
