using System;
using System.Collections.Generic;

namespace SlowestEM.Generator
{
    internal static partial class Enumerable
    {
        public static IEnumerable<TSource> DistinctByKey<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => DistinctByKey(source, keySelector, null);
        public static IEnumerable<TSource> DistinctByKey<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector is null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            if (source == null)
            {
                return new TSource[0];
            }

            return DistinctByIterator(source, keySelector, comparer);
        }

        private static IEnumerable<TSource> DistinctByIterator<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    var set = new HashSet<TKey>(comparer);
                    do
                    {
                        TSource element = enumerator.Current;
                        if (set.Add(keySelector(element)))
                        {
                            yield return element;
                        }
                    }
                    while (enumerator.MoveNext());
                }
            }
        }
    }
}