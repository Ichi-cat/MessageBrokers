namespace MessageBrokerCore.Interfaces
{
    public interface ISerializeService
    {
        byte[] ObjectToBytes<TModel>(TModel model);
        TIn DeserializeObject<TIn>(string request);
        dynamic DeserializeObject(string request, Type parameterType);
    }
}
