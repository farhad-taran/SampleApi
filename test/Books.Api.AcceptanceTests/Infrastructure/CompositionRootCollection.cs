using Xunit;

namespace Books.Api.AcceptanceTests.Infrastructure
{
    /// <summary>
    /// This class is needed so that all the tests use the same composition root without disposing
    /// of and expensive objects
    /// </summary>
    [CollectionDefinition("CompositionRootCollection")]
    public class CompositionRootCollection : ICollectionFixture<CompositionRoot>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}