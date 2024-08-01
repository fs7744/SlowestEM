using System.Data;

namespace SlowestEM
{
    public struct ReaderCacheKey
    {
        public ReaderCacheKey(IDataReader reader)
        {
            Reader = reader;
            hashCode = GetColumnHash(reader);
        }

        private static int GetColumnHash(IDataReader reader, int startBound = 0, int length = -1)
        {
            unchecked
            {
                int max = length < 0 ? reader.FieldCount : startBound + length;
                int hash = (-37 * startBound) + max;
                for (int i = startBound; i < max; i++)
                {
                    object tmp = reader.GetName(i);
                    hash = (-79 * ((hash * 31) + (tmp?.GetHashCode() ?? 0))) + (reader.GetFieldType(i)?.GetHashCode() ?? 0);
                }
                return hash;
            }
        }

        public IDataReader Reader { get; set; }

        private int hashCode;

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}