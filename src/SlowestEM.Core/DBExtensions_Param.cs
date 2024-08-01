using System.Collections.Concurrent;
using System.Data;
using System.Runtime.CompilerServices;

namespace SlowestEM
{
    public static partial class DBExtensions
    {
        public static ConcurrentDictionary<Type, Action<IDbCommand, object>> ParamCache = new();

        public static void CreateParams<T>(this IDbCommand cmd, T data) where T : class
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

        private static readonly object[] s_BoxedInt32 = [-1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        private static readonly object s_BoxedTrue = true, s_BoxedFalse = false;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object AsValue(int value)
            => value >= -1 && value <= 10 ? s_BoxedInt32[value + 1] : value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object AsValue(int? value)
            => value.HasValue ? AsValue(value.GetValueOrDefault()) : DBNull.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object AsValue(bool value)
            => value ? s_BoxedTrue : s_BoxedFalse;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object AsValue(bool? value)
            => value.HasValue ? (value.GetValueOrDefault() ? s_BoxedTrue : s_BoxedFalse) : DBNull.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object AsValue<T>(T? value) where T : struct
            => value.HasValue ? AsValue(value.GetValueOrDefault()) : DBNull.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object AsValue(object? value)
            => value ?? DBNull.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object AsGenericValue<T>(T value)
        {
            // note JIT elide here
            if (typeof(T) == typeof(bool)) return AsValue(Unsafe.As<T, bool>(ref value));
            if (typeof(T) == typeof(bool?)) return AsValue(Unsafe.As<T, bool?>(ref value));
            if (typeof(T) == typeof(int)) return AsValue(Unsafe.As<T, int>(ref value));
            if (typeof(T) == typeof(int?)) return AsValue(Unsafe.As<T, int?>(ref value));
            return AsValue((object?)value);
        }
    }
}