using Newtonsoft.Json;
using HttpClientService.Interfaces;

namespace Challenge02.Infraestructure.Serializer
{
    public class CustomSerializer : ISerialization
    {
        public object Deserialize(string content, Type typeExpected)
        {
            return JsonConvert.DeserializeObject(content, typeExpected);
        }

        public string Serialize<TRequest>(TRequest content)
        {
            return JsonConvert.SerializeObject(content);
        }
    }
}
