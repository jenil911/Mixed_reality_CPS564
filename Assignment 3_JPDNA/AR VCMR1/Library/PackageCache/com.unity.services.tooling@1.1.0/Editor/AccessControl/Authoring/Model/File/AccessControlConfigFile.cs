using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Model.File
{
    [Serializable]
    class AccessControlConfigFile : IAccessControlConfigFile
    {
        [JsonIgnore]
        public string Extension => ProjectAccessFileExtension.Extension;

        [JsonProperty("$schema")]
        public string Schema => "https://ugs-config-schemas.unity3d.com/v1/project-access-policy.schema.json";

        [JsonRequired]
        public List<AccessControlStatement> Statements  { get; set; }

        [JsonIgnore]
        public string FileBodyText
        {
            get
            {
                var goodDefault = new AccessControlConfigFile()
                {
                    Statements = new List<AccessControlStatement>()
                    {
                        AccessControlStatement.GetDefaultStatement()
                    }
                };
                return JsonConvert.SerializeObject(
                    goodDefault,
                    GetSerializationSettings());
            }
        }

        public static JsonSerializerSettings GetSerializationSettings()
        {
            var settings = new JsonSerializerSettings()
            {
                Converters = { new StringEnumConverter() },
                Formatting = Formatting.Indented,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
            };
            return settings;
        }
    }
}
