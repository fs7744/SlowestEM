using System.Collections.Concurrent;
using System.Data;

namespace SlowestEM
{
    public static class DBExtensions
    {
        public static ConcurrentDictionary<Type, Func<IDataReader, object>> ReaderCache = new ConcurrentDictionary<Type, Func<IDataReader, object>>();
        public static ConcurrentDictionary<Type, Func<Type[],Func<IDataReader, object>>> GenericTypeDefinitionReaderCache = new ConcurrentDictionary<Type, Func<Type[], Func<IDataReader, object>>>();
        public static IEnumerable<T> ReadTo<T>(this IDataReader reader) where T : class
        {
            var t = typeof(T);
            if (ReaderCache.TryGetValue(t, out var cache))
            {
                return cache(reader) as IEnumerable<T>;
            }
            else if (t.IsGenericType && GenericTypeDefinitionReaderCache.TryGetValue(t.GetGenericTypeDefinition(), out var makeCache))
            {
                cache = makeCache(t.GetGenericArguments());
                ReaderCache.TryAdd(t, cache);
                return cache(reader) as IEnumerable<T>;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}