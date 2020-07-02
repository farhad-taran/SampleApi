using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Books.Api.AcceptanceTests.Infrastructure;
using Xunit;

namespace Books.Api.AcceptanceTests.Diagnostics
{
    public class DiagnosticsTests : AcceptanceTestFixture
    {
        public DiagnosticsTests(CompositionRoot compositionRoot) : base(compositionRoot)
        {
        }

        [Fact]
        public async Task PingEndpoint_WhenServiceIsRunning_ShouldReturnOk()
        {
            var response = await HttpClient.GetAsync("/api/ping");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync()).Should().Be("Pong");
        }

        [Fact]
        public async Task HealthCheck_Endpoint_WhenServiceIsRunning_ShouldReturnOk()
        {
            var response = await HttpClient.GetAsync("/api/health");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync()).Should().Be("Healthy");
        }
    }
}