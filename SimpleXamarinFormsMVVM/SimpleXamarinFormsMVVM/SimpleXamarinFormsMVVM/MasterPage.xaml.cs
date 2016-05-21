using SimpleXamarinFormsMVVM.Core.View;

namespace SimpleXamarinFormsMVVM
{
    public partial class MasterPage : IView<TestMasterViewModel>
    {
        public MasterPage()
        {
            InitializeComponent();
        }

        public TestMasterViewModel Model { get; set; }
    }
}
