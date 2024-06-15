using MessageBrokerCore.Interfaces;
using System.Text.Json;

namespace MessageBrokerCore.Services
{
    public class SerializeService : ISerializeService
    {
        public byte[] ObjectToBytes<TModel>(TModel model)
        {
            return JsonSerializer.SerializeToUtf8Bytes(model);
        }

        public TIn DeserializeObject<TIn>(string request)
        {
            return JsonSerializer.Deserialize<TIn>(request) ?? throw new Exception("Couldn't deserialize message");
        }

        public dynamic DeserializeObject(string request, Type parameterType)
        {
            return JsonSerializer.Deserialize(request, parameterType) ?? throw new Exception("Couldn't deserialize message");
        }
    }
}
