using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Model.File
{
    interface IAccessControlConfigFile
    {
        string Extension { get; }
        [JsonProperty("$schema")]
        string Schema { get; }
        [JsonRequired]
        List<AccessControlStatement> Statements { get; set; }
        string FileBodyText { get; }
    }
}
