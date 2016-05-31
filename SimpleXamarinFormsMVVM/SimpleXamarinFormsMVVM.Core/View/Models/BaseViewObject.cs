using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using SimpleXamarinFormsMVVM.Core.Collections;
using SimpleXamarinFormsMVVM.Core.Extentions;

namespace SimpleXamarinFormsMVVM.Core.View.Models
{
    public abstract class BaseViewObject : IDisposable
    {
        private readonly StackDisposable disposables = new StackDisposable();

        public event PropertyChangedEventHandler PropertyChanged;

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

        public void Dispose()
        {
            disposables.Dispose();
        }

        protected T AddDisposable<T>(T disposable) where T : IDisposable
        {
            disposables.Push(disposable);
            return disposable;
        }
    }
}
