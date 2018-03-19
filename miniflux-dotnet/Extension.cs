using Newtonsoft.Json;

namespace Miniflux
{
    public static class Extension
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return str == null || str == "";
        }

        public static T AsType<T>(this string json)
        {
            return (T)JsonConvert.DeserializeObject(json, typeof(T));
        }

        public static string ToJson(this object obj)
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}