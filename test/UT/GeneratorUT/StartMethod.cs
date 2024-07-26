namespace UT.GeneratorUT
{
    public enum AEnum : int
    { 
        A
    }

    public class StartMethod
    {
        public string Str { get; set; }
        public string? Str2 { get; set; }
        public int? Int { get; set; }
        public Nullable<int> Int2 { get; set; }
        public AEnum AEnum { get; set; }
        public AEnum? AEnum2 { get; set; }

        [Fact]
        public void CallNoError()
        {
            var s = new { A = "s" };
            SlowestEM.Generator.EntitiesGenerator.Enable();
        }
    }
}
