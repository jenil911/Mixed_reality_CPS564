using Newtonsoft.Json;

namespace Unity.Services.Deployment.Editor.JsonUtils
{
    class NewtonsoftJsonConverter : IJsonConverter
    {
        JsonSerializerSettings m_SerializerSettings =
            new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All, };
        public TResult DeserializeObject<TResult>(string jsonString)
        {
            return JsonConvert.DeserializeObject<TResult>(jsonString, m_SerializerSettings);
        }

        public string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj, m_SerializerSettings);
        }
    }
}
