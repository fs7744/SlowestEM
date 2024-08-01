using System.Collections.Concurrent;
using System.Data;
using System.Runtime.CompilerServices;

namespace SlowestEM
{
    public static partial class DBExtensions
    {
        public static ConcurrentDictionary<Type, Func<IDataReader, object>> ReaderCache = new ConcurrentDictionary<Type, Func<IDataReader, object>>();

        //public static IEnumerable<T> ReadTo<T>(this IDataReader reader) where T : class
        //{
        //    var t = typeof(T);
        //    if (ReaderCache.TryGetValue(t, out var cache))
        //    {
        //        return cache(reader) as IEnumerable<T>;
        //    }
        //    else
        //    {
        //        // todo: emit generate
        //        throw new NotImplementedException();
        //    }
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static string ReadToString(this IDataReader reader, int i)
        //{
        //    return reader.GetString(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static string ReadToStringConvert(this IDataReader reader, int i)
        //{
        //    return reader.GetValue(i)?.ToString();
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static short ReadToInt16(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return reader.GetInt16(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static short ReadToInt16Convert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return Convert.ToInt16(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static short? ReadToInt16Nullable(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return reader.GetInt16(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static short? ReadToInt16NullableConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return Convert.ToInt16(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static ushort ReadToUInt16(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return (UInt16)reader.GetValue(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static ushort ReadToUInt16Convert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return Convert.ToUInt16(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static ushort? ReadToUInt16Nullable(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return (UInt16)reader.GetValue(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static ushort? ReadToUInt16NullableConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return Convert.ToUInt16(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static int ReadToInt32(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return reader.GetInt32(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static int ReadToInt32Convert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return Convert.ToInt32(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static int? ReadToInt32Nullable(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return reader.GetInt32(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static int? ReadToInt32NullableConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return Convert.ToInt32(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static uint ReadToUInt32(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return (uint)reader.GetValue(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static uint ReadToUInt32Convert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return Convert.ToUInt32(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static uint? ReadToUInt32Nullable(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return (uint)reader.GetValue(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static uint? ReadToUInt32NullableConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return Convert.ToUInt32(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static long ReadToInt64(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return reader.GetInt64(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static long ReadToInt64Convert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return Convert.ToInt64(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static long? ReadToInt64Nullable(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return reader.GetInt64(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static long? ReadToInt64NullableConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return Convert.ToInt64(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static ulong ReadToUInt64(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return (ulong)reader.GetValue(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static ulong ReadToUInt64Convert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return Convert.ToUInt64(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static ulong? ReadToUInt64Nullable(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return (ulong)reader.GetValue(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static ulong? ReadToUInt64NullableConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return Convert.ToUInt64(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static float ReadToFloat(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return reader.GetFloat(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static float ReadToFloatConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return Convert.ToSingle(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static float? ReadToFloatNullable(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return reader.GetFloat(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static float? ReadToFloatNullableConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return Convert.ToSingle(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static double ReadToDouble(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return reader.GetDouble(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static double ReadToDoubleConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return Convert.ToDouble(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static double? ReadToDoubleNullable(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return reader.GetDouble(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static double? ReadToDoubleNullableConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return Convert.ToDouble(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static decimal ReadToDecimal(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return reader.GetDecimal(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static decimal ReadToDecimalConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return Convert.ToDecimal(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static decimal? ReadToDecimalNullable(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return reader.GetDecimal(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static decimal? ReadToDecimalNullableConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return Convert.ToDecimal(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static bool ReadToBoolean(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return reader.GetBoolean(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static bool ReadToBooleanConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return Convert.ToBoolean(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static bool? ReadToBooleanNullable(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return reader.GetBoolean(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static bool? ReadToBooleanNullableConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return Convert.ToBoolean(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static char ReadToChar(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return reader.GetChar(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static char ReadToCharConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return Convert.ToChar(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static char? ReadToCharNullable(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return reader.GetChar(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static char? ReadToCharNullableConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return Convert.ToChar(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static byte ReadToByte(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return reader.GetByte(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static byte ReadToByteConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return Convert.ToByte(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static byte? ReadToByteNullable(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return reader.GetByte(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static byte? ReadToByteNullableConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return Convert.ToByte(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static DateTime ReadToDateTime(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return reader.GetDateTime(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static DateTime ReadToDateTimeConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return Convert.ToDateTime(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static DateTime? ReadToDateTimeNullable(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return reader.GetDateTime(i);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static DateTime? ReadToDateTimeNullableConvert(this IDataReader reader, int i)
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return Convert.ToDateTime(reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static T ReadToEnumString<T>(this IDataReader reader, int i) where T : struct
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return Enum.Parse<T>(reader.GetString(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static T ReadToEnum<T>(this IDataReader reader, int i) where T : struct
        //{
        //    if (reader.IsDBNull(i)) return default;
        //    else return (T)Enum.ToObject(typeof(T), reader.GetValue(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static T? ReadToEnumStringNullable<T>(this IDataReader reader, int i) where T : struct
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return Enum.Parse<T>(reader.GetString(i));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static T? ReadToEnumNullable<T>(this IDataReader reader, int i) where T : struct
        //{
        //    if (reader.IsDBNull(i)) return null;
        //    else return (T)Enum.ToObject(typeof(T), reader.GetValue(i));
        //}
    }
}