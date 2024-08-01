using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.IO;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Json;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Model.File;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.IO
{
    class AccessControlResourcesLoader : IAccessControlResourcesLoader
    {
        readonly IFileSystem m_FileSystem;
        readonly IJsonConverter m_JsonConverter;

        public AccessControlResourcesLoader(
            IFileSystem fileSystem,
            IJsonConverter jsonConverter)
        {
            m_FileSystem = fileSystem;
            m_JsonConverter = jsonConverter;
        }

        public async Task LoadFileAsync(
            IProjectAccessFile projectAccessFile,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var fileText = await m_FileSystem.ReadAllText(projectAccessFile.Path, cancellationToken);
                var result = m_JsonConverter.DeserializeObject<AccessControlConfigFile>(fileText);
                projectAccessFile.Statements = result.Statements ?? new List<AccessControlStatement>();
            }
            catch (Exception e)
            {
                projectAccessFile.States.Add(Statuses.FailedToLoadErrorAssetState(e.Message));
            }
        }
    }
}
