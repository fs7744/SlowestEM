namespace UT.GeneratorUT
{
    public class StartMethod
    {
        public string Str { get; set; }
        public string? Str2 { get; set; }
        public int? Int { get; set; }
        public Nullable<int> Int2 { get; set; }

        [Fact]
        public void CallNoError()
        {
            var s = new { A = "s" };
            SlowestEM.Generator.EntitiesGenerator.Enable();
        }
    }
}
