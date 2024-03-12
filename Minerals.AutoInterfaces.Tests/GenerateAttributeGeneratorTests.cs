namespace Minerals.AutoInterfaces.Tests
{
    public class GenerateInterfaceGeneratorTests
    {
        [Fact]
        public Task Attribute_ShouldGenerate()
        {
            return TestHelpers.VerifyGenerator(new GenerateAttributeGenerator(), []);
        }
    }
}