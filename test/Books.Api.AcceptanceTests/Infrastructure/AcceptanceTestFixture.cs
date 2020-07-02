using System.Net.Http;
using AutoFixture;
using Xunit;

namespace Books.Api.AcceptanceTests.Infrastructure
{
    [Collection("CompositionRootCollection")]
    public class AcceptanceTestFixture
    {
        protected Fixture Fixture;
        protected HttpClient HttpClient;
        
        public AcceptanceTestFixture(CompositionRoot compositionRoot)
        {
            Fixture = new Fixture();
            HttpClient = compositionRoot.HttpClient;
        }
    }

    public class AcceptanceTestFixture<TFacade> : AcceptanceTestFixture
        where TFacade : class
    {
        protected TFacade Facade;

        public AcceptanceTestFixture(CompositionRoot compositionRoot) : base(compositionRoot)
        {
            Facade = compositionRoot.GetRequiredService<TFacade>();
        }
    }
}