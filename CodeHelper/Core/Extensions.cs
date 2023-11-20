using Newtonsoft.Json;

namespace CodeHelper.Core
{
    public static class Extensions
    {
        private static readonly JsonSerializerSettings _settings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public static string ToJson<T>(this T value)
        {
            return JsonConvert.SerializeObject(value, Formatting.None, _settings);
        }

        public static T FromJson<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value, _settings);
        }
    }
}
