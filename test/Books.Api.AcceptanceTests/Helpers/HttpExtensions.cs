using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;

namespace Books.Api.AcceptanceTests.Helpers
{
    public static class HttpExtensions
    {
        public static async Task<T> ReadContentAs<T>(this HttpResponseMessage response)
        {
            var stringBody = await response.Content.ReadAsStringAsync();

            var body = stringBody.MapFromJson<T>();

            body.Should().BeOfType<T>(body.MapToJson());

            return body;
        }
    }
}