namespace Minerals.AutoInterfaces.Tests.Attributes
{
    [TestClass]
    public class GenerateInterfaceAttributeGeneratorTests : VerifyBase
    {
        public GenerateInterfaceAttributeGeneratorTests()
        {
            VerifyExtensions.InitializeGlobalSettings();
        }

        [TestMethod]
        public Task Attribute_ShouldGenerate()
        {
            return this.VerifyIncrementalGenerators(new GenerateInterfaceAttributeGenerator());
        }
    }
}