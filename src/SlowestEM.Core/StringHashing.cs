namespace SlowestEM
{
    public static partial class StringHashing
    {
        public static uint NormalizedHash(string value)
        {
            uint hash = 0;
            if (!string.IsNullOrEmpty(value))
            {   // borrowed from Roslyn's switch on string implementation
                hash = 2166136261u;
                foreach (char c in value)
                {
                    if (c == '_' || char.IsWhiteSpace(c)) continue;
                    hash = (char.ToLower(c) ^ hash) * 16777619;
                }
            }
            return hash;
        }
    }
}