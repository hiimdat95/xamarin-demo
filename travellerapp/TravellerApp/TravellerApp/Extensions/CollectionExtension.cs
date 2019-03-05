using System.Collections.Generic;

namespace Xamarin.Forms.Extensions
{
    public static class CollectionExtension
    {
        public static void AddRange<T>(this ICollection<T> self, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                self.Add(item);
            }
        }
    }
}