using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
            DefaultContractResolver contractResolver = new()
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            var settings = new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}