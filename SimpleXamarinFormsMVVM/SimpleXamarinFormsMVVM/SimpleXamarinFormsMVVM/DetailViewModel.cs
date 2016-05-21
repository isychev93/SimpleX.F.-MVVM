using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleXamarinFormsMVVM.Core.View.Services;

namespace SimpleXamarinFormsMVVM
{
    public class DetailViewModel : Core.View.Models.DetailViewModel
    {
        public DetailViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public int Order { get; set; }
    }
}
