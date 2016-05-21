using SimpleXamarinFormsMVVM.Core.View;

namespace SimpleXamarinFormsMVVM
{
    public partial class StartView : IView<StartViewModel>
    {
        public StartView()
        {
            InitializeComponent();
        }

        public StartViewModel Model { get; set; }
    }
}
