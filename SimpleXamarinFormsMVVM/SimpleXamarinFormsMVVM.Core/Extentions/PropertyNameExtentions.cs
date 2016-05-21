using System;
using System.Linq.Expressions;

namespace SimpleXamarinFormsMVVM.Core.Extentions
{
    public static class PropertyNameExtentions
    {
        public static string GetPropertyName<T>(this Expression<Func<T>> expression)
        {
            var unaryExpression = expression.Body as UnaryExpression;
            MemberExpression memberExpression;
            if (unaryExpression == null)
                memberExpression = expression.Body as MemberExpression;
            else
                memberExpression = unaryExpression.Operand as MemberExpression;

            if (memberExpression == null)
                throw new ArgumentException(string.Format("Wrong expression: {0}", expression));

            var propertyInfo = memberExpression.Member;
            if (propertyInfo == null)
                throw new ArgumentException(string.Format("Wrong expression: {0}", expression));

            return propertyInfo.Name;
        }
    }
}
