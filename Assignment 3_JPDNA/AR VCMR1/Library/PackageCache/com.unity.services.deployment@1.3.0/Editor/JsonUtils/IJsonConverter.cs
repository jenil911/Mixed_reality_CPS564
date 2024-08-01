namespace Unity.Services.Deployment.Editor.JsonUtils
{
    interface IJsonConverter
    {
        TResult DeserializeObject<TResult>(string jsonString);
        string SerializeObject(object obj);
    }
}
