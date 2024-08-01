namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Json
{
    interface IJsonConverter
    {
        T DeserializeObject<T>(string value, bool matchCamelCaseFieldName = false);
        string SerializeObject<T>(T obj);
    }
}
