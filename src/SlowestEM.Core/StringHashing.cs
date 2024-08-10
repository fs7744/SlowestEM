using System.Runtime.CompilerServices;

namespace SlowestEM
{
    public static partial class StringHashing
    {
        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "GetNonRandomizedHashCodeOrdinalIgnoreCase")]
        public static extern int NormalizedHash(this string c);
    }
}