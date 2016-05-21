using SimpleXamarinFormsMVVM.Core.View;

namespace SimpleXamarinFormsMVVM
{
    public partial class DetailView : IView<DetailViewModel>
    {
        public DetailView()
        {
            InitializeComponent();
        }

        public DetailViewModel Model { get; set; }
    }
}
