using System.Collections.Concurrent;
using System.Data;

namespace SlowestEM
{
    public static class ClassReaderCache<T>
    {
        public static readonly ConcurrentDictionary<ReaderCacheKey, Action<T, IDataReader>[]> Cache = new ();
    }
}