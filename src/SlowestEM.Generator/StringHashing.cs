using System;

namespace SlowestEM.Generator
{
    public static partial class StringHashing
    {


        private static Func<string, int> hash;
        static StringHashing()
        {
            //hash = (Func<string, int>)typeof(string).GetMethod("GetNonRandomizedHashCodeOrdinalIgnoreCase", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).CreateDelegate(typeof(Func<string, int>));
        }

        public static int NormalizedHash(string value)
        {
            return 0;
            return hash(value);
        }
    }
}