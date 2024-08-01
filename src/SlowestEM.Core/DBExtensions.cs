using System.Collections.Concurrent;
using System.Data;

namespace SlowestEM
{
    public static class DBExtensions
    {
        public static ConcurrentDictionary<Type, Func<IDataReader, object>> ReaderCache = new ConcurrentDictionary<Type, Func<IDataReader, object>>();

        public static ConcurrentDictionary<Type, Action<IDbCommand, object>> ParamCache = new ();

        public static IEnumerable<T> ReadTo<T>(this IDataReader reader) where T : class
        {
            var t = typeof(T);
            if (ReaderCache.TryGetValue(t, out var cache))
            {
                return cache(reader) as IEnumerable<T>;
            }
            else
            {
                // todo: emit generate
                throw new NotImplementedException();
            }
        }

        public static void CreateParams<T>(this T data, IDbCommand cmd) where T : class
        {
            var t = typeof(T);
            if (ParamCache.TryGetValue(t, out var cache))
            {
                cache(cmd, data);
            }
            else
            {
                // todo: emit generate
                throw new NotImplementedException();
            }
        }

        public static string ReadToString(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                return reader.GetValue(i)?.ToString();
            }
            else
            {
                return reader.GetString(i);
            }
        }

        public static short ReadToInt16(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return default;
                else return Convert.ToInt16(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return default;
                else return reader.GetInt16(i);
            }
        }

        public static short? ReadToInt16Nullable(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return null;
                else return Convert.ToInt16(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return null;
                else return reader.GetInt16(i);
            }
        }

        public static ushort ReadToUInt16(this IDataReader reader, int i, bool needConvert)
        {
            if (reader.IsDBNull(i)) return default;
            else return Convert.ToUInt16(reader.GetValue(i));
        }

        public static ushort? ReadToUInt16Nullable(this IDataReader reader, int i, bool needConvert)
        {
            if (reader.IsDBNull(i)) return null;
            else return Convert.ToUInt16(reader.GetValue(i));
        }

        public static int ReadToInt32(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return default;
                else return Convert.ToInt32(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return default;
                else return reader.GetInt32(i);
            }
        }

        public static int? ReadToInt32Nullable(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return null;
                else return Convert.ToInt32(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return null;
                else return reader.GetInt32(i);
            }
        }

        public static uint ReadToUInt32(this IDataReader reader, int i, bool needConvert)
        {
            if (reader.IsDBNull(i)) return default;
            else return Convert.ToUInt32(reader.GetValue(i));
        }

        public static uint? ReadToUInt32Nullable(this IDataReader reader, int i, bool needConvert)
        {
            if (reader.IsDBNull(i)) return null;
            else return Convert.ToUInt32(reader.GetValue(i));
        }

        public static long ReadToInt64(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return default;
                else return Convert.ToInt64(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return default;
                else return reader.GetInt64(i);
            }
        }

        public static long? ReadToInt64Nullable(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return null;
                else return Convert.ToInt64(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return null;
                else return reader.GetInt64(i);
            }
        }

        public static ulong ReadToUInt64(this IDataReader reader, int i, bool needConvert)
        {
            if (reader.IsDBNull(i)) return default;
            else return Convert.ToUInt64(reader.GetValue(i));
        }

        public static ulong? ReadToUInt64Nullable(this IDataReader reader, int i, bool needConvert)
        {
            if (reader.IsDBNull(i)) return null;
            else return Convert.ToUInt64(reader.GetValue(i));
        }

        public static float ReadToFloat(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return default;
                else return Convert.ToSingle(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return default;
                else return reader.GetFloat(i);
            }
        }

        public static float? ReadToFloatNullable(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return null;
                else return Convert.ToSingle(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return null;
                else return reader.GetFloat(i);
            }
        }

        public static double ReadToDouble(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return default;
                else return Convert.ToDouble(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return default;
                else return reader.GetDouble(i);
            }
        }

        public static double? ReadToDoubleNullable(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return null;
                else return Convert.ToDouble(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return null;
                else return reader.GetDouble(i);
            }
        }

        public static decimal ReadToDecimal(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return default;
                else return Convert.ToDecimal(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return default;
                else return reader.GetDecimal(i);
            }
        }

        public static decimal? ReadToDecimalNullable(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return null;
                else return Convert.ToDecimal(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return null;
                else return reader.GetDecimal(i);
            }
        }

        public static bool ReadToBoolean(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return default;
                else return Convert.ToBoolean(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return default;
                else return reader.GetBoolean(i);
            }
        }

        public static bool? ReadToBooleanNullable(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return null;
                else return Convert.ToBoolean(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return null;
                else return reader.GetBoolean(i);
            }
        }

        public static char ReadToChar(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return default;
                else return Convert.ToChar(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return default;
                else return reader.GetChar(i);
            }
        }

        public static char? ReadToCharNullable(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return null;
                else return Convert.ToChar(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return null;
                else return reader.GetChar(i);
            }
        }

        public static byte ReadToByte(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return default;
                else return Convert.ToByte(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return default;
                else return reader.GetByte(i);
            }
        }

        public static byte? ReadToByteNullable(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return null;
                else return Convert.ToByte(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return null;
                else return reader.GetByte(i);
            }
        }

        public static DateTime ReadToDateTime(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return default;
                else return Convert.ToDateTime(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return default;
                else return reader.GetDateTime(i);
            }
        }

        public static DateTime? ReadToDateTimeNullable(this IDataReader reader, int i, bool needConvert)
        {
            if (needConvert)
            {
                if (reader.IsDBNull(i)) return null;
                else return Convert.ToDateTime(reader.GetValue(i));
            }
            else
            {
                if (reader.IsDBNull(i)) return null;
                else return reader.GetDateTime(i);
            }
        }

        public static T ReadToEnum<T>(this IDataReader reader, int i)
        {
            if (reader.IsDBNull(i)) return default;
            else return (T)Enum.ToObject(typeof(T), reader.GetValue(i));
        }

        public static T? ReadToEnumNullable<T>(this IDataReader reader, int i) where T : struct
        {
            if (reader.IsDBNull(i)) return null;
            else return (T)Enum.ToObject(typeof(T), reader.GetValue(i));
        }
    }
}