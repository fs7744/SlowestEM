namespace UT.GeneratorUT
{
    public class StartMethod
    {
        [Fact]
        public void CallNoError()
        {
            SlowestEM.Generator.EntitiesGenerator.Enable();

            var a = SlowestEM.Generator.StartMethod_Accessors.Ctor();
        }
    }
}
