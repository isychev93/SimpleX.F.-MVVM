using System.Collections.Generic;

namespace SimpleXamarinFormsMVVM.Core.Extentions
{
    public static class EnumerableExtentions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> itemsToAdd)
        {
            foreach (var newItem in itemsToAdd)
                list.Add(newItem);
        }
    }
}
