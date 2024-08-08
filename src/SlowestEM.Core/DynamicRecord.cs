using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SlowestEM
{
    internal readonly struct DynamicRecordField
    {
        public DynamicRecordField(string name, Type type, string dataTypeName)
        {
            Name = name;
            Type = type;
            DataTypeName = dataTypeName;
        }
        public readonly Type Type;
        public readonly string Name, DataTypeName;
    }

    internal sealed class DynamicRecord : DbDataRecord, IReadOnlyDictionary<string, object?>, IDictionary<string, object?>,
    IDynamicMetaObjectProvider
    {
        private readonly DynamicRecordField[] fields;
        private readonly Dictionary<string, int> keys;
        private readonly object[] values;
        public DynamicRecord(DynamicRecordField[] fields, IDataReader source, Dictionary<string, int> keys)
        {
            this.fields = fields;
            this.keys = keys;
            values = new object[fields.Length];
            source.GetValues(values);
        }

        public override int GetOrdinal(string name)
        {
            return keys[name];
        }

        public override int GetValues(object[] values)
        {
            var count = Math.Max(values.Length, FieldCount);
            Array.Copy(this.values, values, count);
            return count;
        }

        public override int FieldCount => fields.Length;

        public IEnumerable<string> Keys => keys.Keys;

        public IEnumerable<object> Values => values;

        public int Count => FieldCount;
        ICollection<string> IDictionary<string, object?>.Keys => keys.Keys;

        ICollection<object?> IDictionary<string, object?>.Values => values;

        bool ICollection<KeyValuePair<string, object?>>.IsReadOnly => true;

        object? IDictionary<string, object?>.this[string key]
        {
            get => this[key];
            set => throw new NotSupportedException();
        }
        public override string GetName(int i) => fields[i].Name;
        public override Type GetFieldType(int i) => fields[i].Type;

        protected override DbDataReader GetDbDataReader(int i) => throw new NotSupportedException();

        public override object GetValue(int i) => values[i];
        public override object this[string name]
        {
            get
            {
                return values[keys[name]];
            }
        }
        public override object this[int i] => values[i];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T As<T>(object? value)
        {
            if (value is null or DBNull)
            {
                // if value-typed and *not* Nullable<T>, then: that's an error
                if (typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) is null)
                {
                    throw new ArgumentNullException("value");
                }
                return default!;
            }
            else
            {
                if (value is T typed)
                {
                    return typed;
                }
                string? s;
                // note we're using value-type T JIT dead-code removal to elide most of these checks
                if (typeof(T) == typeof(int))
                {
                    int t = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<int, T>(ref t);
                }
                if (typeof(T) == typeof(int?))
                {
                    int? t = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<int?, T>(ref t);
                }
                else if (typeof(T) == typeof(bool))
                {
                    bool t = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<bool, T>(ref t);
                }
                else if (typeof(T) == typeof(bool?))
                {
                    bool? t = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<bool?, T>(ref t);
                }
                else if (typeof(T) == typeof(float))
                {
                    float t = Convert.ToSingle(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<float, T>(ref t);
                }
                else if (typeof(T) == typeof(float?))
                {
                    float? t = Convert.ToSingle(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<float?, T>(ref t);
                }
                else if (typeof(T) == typeof(double))
                {
                    double t = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<double, T>(ref t);
                }
                else if (typeof(T) == typeof(double?))
                {
                    double? t = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<double?, T>(ref t);
                }
                else if (typeof(T) == typeof(decimal))
                {
                    decimal t = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<decimal, T>(ref t);
                }
                else if (typeof(T) == typeof(decimal?))
                {
                    decimal? t = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<decimal?, T>(ref t);
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    DateTime t = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<DateTime, T>(ref t);
                }
                else if (typeof(T) == typeof(DateTime?))
                {
                    DateTime? t = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<DateTime?, T>(ref t);
                }
                else if (typeof(T) == typeof(Guid) && (s = value as string) is not null)
                {
                    Guid t = Guid.Parse(s);
                    return Unsafe.As<Guid, T>(ref t);
                }
                else if (typeof(T) == typeof(Guid?) && (s = value as string) is not null)
                {
                    Guid? t = Guid.Parse(s);
                    return Unsafe.As<Guid?, T>(ref t);
                }
                else if (typeof(T) == typeof(long))
                {
                    long t = Convert.ToInt64(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<long, T>(ref t);
                }
                else if (typeof(T) == typeof(long?))
                {
                    long? t = Convert.ToInt64(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<long?, T>(ref t);
                }
                else if (typeof(T) == typeof(short))
                {
                    short t = Convert.ToInt16(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<short, T>(ref t);
                }
                else if (typeof(T) == typeof(short?))
                {
                    short? t = Convert.ToInt16(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<short?, T>(ref t);
                }
                else if (typeof(T) == typeof(sbyte))
                {
                    sbyte t = Convert.ToSByte(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<sbyte, T>(ref t);
                }
                else if (typeof(T) == typeof(sbyte?))
                {
                    sbyte? t = Convert.ToSByte(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<sbyte?, T>(ref t);
                }
                else if (typeof(T) == typeof(ulong))
                {
                    ulong t = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<ulong, T>(ref t);
                }
                else if (typeof(T) == typeof(ulong?))
                {
                    ulong? t = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<ulong?, T>(ref t);
                }
                else if (typeof(T) == typeof(uint))
                {
                    uint t = Convert.ToUInt32(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<uint, T>(ref t);
                }
                else if (typeof(T) == typeof(uint?))
                {
                    uint? t = Convert.ToUInt32(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<uint?, T>(ref t);
                }
                else if (typeof(T) == typeof(ushort))
                {
                    ushort t = Convert.ToUInt16(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<ushort, T>(ref t);
                }
                else if (typeof(T) == typeof(ushort?))
                {
                    ushort? t = Convert.ToUInt16(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<ushort?, T>(ref t);
                }
                else if (typeof(T) == typeof(byte))
                {
                    byte t = Convert.ToByte(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<byte, T>(ref t);
                }
                else if (typeof(T) == typeof(byte?))
                {
                    byte? t = Convert.ToByte(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<byte?, T>(ref t);
                }
                else if (typeof(T) == typeof(char))
                {
                    char t = Convert.ToChar(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<char, T>(ref t);
                }
                else if (typeof(T) == typeof(char?))
                {
                    char? t = Convert.ToChar(value, CultureInfo.InvariantCulture);
                    return Unsafe.As<char?, T>(ref t);
                }
                // this won't elide, but: we'll live with it
                else if (typeof(T) == typeof(string))
                {
                    var t = Convert.ToString(value, CultureInfo.InvariantCulture)!;
                    return Unsafe.As<string, T>(ref t);
                }
                else
                {
                    return (T)Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T), CultureInfo.InvariantCulture);
                }
            }
        }

        public override bool GetBoolean(int i) => As<bool>(i);
        public override char GetChar(int i) => As<char>(i);
        public override string GetString(int i) => As<string>(i);
        public override byte GetByte(int i) => As<byte>(i);
        public override DateTime GetDateTime(int i) => As<DateTime>(i);
        public override decimal GetDecimal(int i) => As<decimal>(i);
        public override double GetDouble(int i) => As<double>(i);
        public override float GetFloat(int i) => As<float>(i);
        public override Guid GetGuid(int i) => As<Guid>(i);
        public override short GetInt16(int i) => As<short>(i);
        public override int GetInt32(int i) => As<int>(i);
        public override long GetInt64(int i) => As<long>(i);
        public override bool IsDBNull(int i) => values[i] is DBNull or null;
        public override string GetDataTypeName(int i) => fields[i].DataTypeName;

        static int CheckOffsetAndComputeLength(int totalLength, long dataIndex, ref int length)
        {
            var offset = checked((int)dataIndex);
            var remaining = totalLength - offset;
            length = Math.Clamp(remaining, 0, length);
            return offset;
        }

        public override long GetBytes(int i, long dataIndex, byte[]? buffer, int bufferIndex, int length)
        {
            if (buffer is null) return 0;
            byte[] blob = (byte[])values[i];
            Buffer.BlockCopy(blob, CheckOffsetAndComputeLength(blob.Length, dataIndex, ref length), buffer, bufferIndex, length);
            return length;
        }
        public override long GetChars(int i, long dataIndex, char[]? buffer, int bufferIndex, int length)
        {
            if (buffer is null) return 0;
            if (values[i] is string s)
            {
                s.CopyTo(CheckOffsetAndComputeLength(s.Length, dataIndex, ref length), buffer, bufferIndex, length);
            }
            else
            {
                char[] clob = (char[])values[i];
                Array.Copy(clob, CheckOffsetAndComputeLength(clob.Length, dataIndex, ref length), buffer, bufferIndex, length);
            }
            return length;
        }

        public bool ContainsKey(string key) => keys.ContainsKey(key);

        public bool TryGetValue(string key, out object? value)
        {
            if (keys.TryGetValue(key, out var i))
            {
                value = values[i];
                return true;
            }
            value = null;
            return false;
        }

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            for (int i = 0; i < FieldCount; i++)
            {
                yield return new(fields[i].Name, values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void IDictionary<string, object?>.Add(string key, object? value) => throw new NotSupportedException();

        bool IDictionary<string, object?>.Remove(string key) => throw new NotSupportedException();

        void ICollection<KeyValuePair<string, object?>>.Add(KeyValuePair<string, object?> item) => throw new NotSupportedException();

        void ICollection<KeyValuePair<string, object?>>.Clear() => throw new NotSupportedException();

        bool ICollection<KeyValuePair<string, object?>>.Contains(KeyValuePair<string, object?> item)
        => TryGetValue(item.Key, out var value) && Equals(value, item.Value);

        void ICollection<KeyValuePair<string, object?>>.CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
        {
            for (int i = 0; i < FieldCount; i++)
            {
                array[arrayIndex++] = new(fields[i].Name, values[i]);
            }
        }

        bool ICollection<KeyValuePair<string, object?>>.Remove(KeyValuePair<string, object?> item) => throw new NotSupportedException();

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
            => new DynamicRecordMetaObject(parameter, BindingRestrictions.Empty, this);

        private sealed class DynamicRecordMetaObject : DynamicMetaObject
        {
            private static readonly MethodInfo getValueMethod;
            static DynamicRecordMetaObject()
            {
                IReadOnlyDictionary<string, object> tmp = new Dictionary<string, object> { { "", "" } };
                _ = tmp[""]; // to ensure the indexer is not trimmed away
                getValueMethod = typeof(IReadOnlyDictionary<string, object>).GetProperty("Item")?.GetGetMethod()
                    ?? throw new InvalidOperationException("Unable to resolve indexer");
            }

            public DynamicRecordMetaObject(
            Expression expression,
            BindingRestrictions restrictions,
            object value
            )
            : base(expression, restrictions, value)
            {
            }

            private DynamicMetaObject CallMethod(
            MethodInfo method,
            Expression[] parameters
            )
            {
                var callMethod = new DynamicMetaObject(
                    Expression.Call(
                        Expression.Convert(Expression, LimitType),
                        method,
                        parameters),
                    BindingRestrictions.GetTypeRestriction(Expression, LimitType)
                    );
                return callMethod;
            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                var parameters = new Expression[] { Expression.Constant(binder.Name) };

                var callMethod = CallMethod(getValueMethod, parameters);

                return callMethod;
            }

            public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
                => throw new NotSupportedException("Dynamic records are considered read-only currently");

            // Needed for Visual basic dynamic support
            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            {
                var parameters = new Expression[]
                {
                Expression.Constant(binder.Name)
                };

                var callMethod = CallMethod(getValueMethod, parameters);

                return callMethod;
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                if (HasValue && Value is IDictionary<string, object> lookup) return lookup.Keys;
                return Array.Empty<string>();
            }
        }
    }

    public class DynamicRecordFactory<T> where T : class
    {
        public static readonly DynamicRecordFactory<T> Instance = new();

        public IEnumerable<T> Read(IDataReader reader)
        {
            var arr = new DynamicRecordField[reader.FieldCount];
            var dict = new Dictionary<string, int>();
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = new DynamicRecordField(reader.GetName(i), reader.GetFieldType(i), reader.GetDataTypeName(i));
                dict[arr[i].Name] = i;
            }
            
            try
            {
                while (reader.Read())
                {
                    yield return (T)(object)new DynamicRecord(arr, reader, dict);
                }
            }
            finally
            {
                reader.Dispose();
            }
        }
    }
}
