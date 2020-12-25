using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility
{
    public sealed class IndexedItem<T>
    {
        public IndexedItem(int index, T value)
        {
            Index = index;
            Value = value;
        }

        public int Index { get; }
        public T Value { get; }
    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> AddItem<T>(this IEnumerable<T> items, T item)
        {
            var collection = (items ?? Enumerable.Empty<T>()).ToList();
            collection.Add(item);
            return collection;
        }

        public static bool IsUnique<T,P>(this IEnumerable<T> items, Func<T,P> getter, Func<P,bool> isEmpty = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (getter == null)
                throw new ArgumentNullException(nameof(getter));

            var props = items.Select(t => getter(t));
            if (isEmpty != null)
                props = props.Where(p => !isEmpty(p));

            return props.GroupBy(p => p).All(g => g.Count() == 1);
        }

        public static IEnumerable<IndexedItem<T>> GetIndexed<T>(this IEnumerable<T> items)
        {
            var counter = 0;
            return (items ?? Enumerable.Empty<T>()).Select(t => new IndexedItem<T>(counter++, t)).ToArray();
        }

        public static string GetContextKey<T>( this IndexedItem<T> item, string outerContext)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return String.IsNullOrEmpty(outerContext) ? item.Index.ToString() : $"{item.Index}:{outerContext}";
        }

    }
}
