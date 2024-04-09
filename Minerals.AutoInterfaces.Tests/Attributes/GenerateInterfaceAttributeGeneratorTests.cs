namespace Minerals.AutoInterfaces.Tests.Attributes
{
    [TestClass]
    public class GenerateInterfaceAttributeGeneratorTests : VerifyBase
    {
        public GenerateInterfaceAttributeGeneratorTests()
        {
            var references = VerifyExtensions.GetAppReferences
            (
                typeof(object),
                typeof(GenerateInterfaceAttributeGenerator)
            );
            VerifyExtensions.Initialize(references);
        }

        [TestMethod]
        public Task Attribute_ShouldGenerate()
        {
            return this.VerifyIncrementalGenerators(new GenerateInterfaceAttributeGenerator());
        }
    }
}