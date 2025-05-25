namespace Minerals.AutoInterfaces.GenerationTests
{
    [UsesVerify, TestClass]
    public partial class AutoInterfaceAttributeGeneratorTests
    {
        [TestMethod]
        public Task AttributeGeneration()
        {
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator>(string.Empty);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }
    }
}