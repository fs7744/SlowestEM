using System;

namespace SlowestEM.Generator
{
    public static partial class StringHashing1
    {
        private static Func<string, int> hash;
        static StringHashing1()
        {
            hash = (Func<string, int>)typeof(string).GetMethod("GetNonRandomizedHashCodeOrdinalIgnoreCase", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).CreateDelegate(typeof(Func<string, int>));
        }

        public static int NormalizedHash(string value)
        {
            return hash(value);
        }
    }
}