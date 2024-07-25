using System.Collections.Concurrent;
using System.Data;

namespace SlowestEM
{
    public static class DBExtensions
    {
        public static ConcurrentDictionary<Type, Func<IDataReader, object>> ReaderCache = new ConcurrentDictionary<Type, Func<IDataReader, object>>();

        public static IEnumerable<T> ReadTo<T>(this IDataReader reader) where T : class
        {
            if (ReaderCache.TryGetValue(typeof(T), out var cache))
            {
                return cache(reader) as IEnumerable<T>;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}