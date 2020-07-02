using Newtonsoft.Json;
using Books.Api.Extensions;

namespace Books.Api.AcceptanceTests.Helpers
{
    public static class ObjectExtensions
    {
        public static string MapToJson<T>(this T o)
        {
            return JsonConvert.SerializeObject(o);
        }

        public static T MapFromJson<T>(this string o)
        {
            return JsonConvert.DeserializeObject<T>(o);
        }
    }
}