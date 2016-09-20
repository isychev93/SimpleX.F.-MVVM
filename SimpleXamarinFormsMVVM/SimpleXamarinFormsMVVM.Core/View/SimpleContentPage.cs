using System;
using SimpleXamarinFormsMVVM.Core.View.Models;
using Xamarin.Forms;

namespace SimpleXamarinFormsMVVM.Core.View
{
    public class SimpleContentPage : ContentPage
    {
        protected override bool OnBackButtonPressed()
        {
            var pageModel = BindingContext as IViewModel;
            if (pageModel == null)
                throw new InvalidOperationException("SimpleContentPage must use IViewModel model");

            pageModel.GoBackCommand.Execute(null);
            return true;
        }
    }
}
