using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SimpleXamarinFormsMVVM.Core.Extentions;
using SimpleXamarinFormsMVVM.Core.View.Services;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View.Models
{
    public abstract class BaseViewModel : INotifyPropertyChanged, IViewModel
    {
        protected readonly INavigationService navigationService;

        protected BaseViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            GoBackCommand = new Command(() => navigationService.GoBack());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual bool ShowInNewNavigationPage
        {
            get { return false; }
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(expression.GetPropertyName()));
        }

        public static string PropertyName<T>(Expression<Func<T>> expression)
        {
            return expression.GetPropertyName();
        }

        public virtual ICommand GoBackCommand { get; }
    }
}
