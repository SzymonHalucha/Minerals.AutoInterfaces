namespace Minerals.AutoInterfaces.Tests
{
    public class GenerateInterfaceAttributeGeneratorTests
    {
        [Fact]
        public Task Attribute_ShouldGenerate()
        {
            return TestHelpers.VerifyGenerator(new GenerateInterfaceAttributeGenerator(), []);
        }
    }
}