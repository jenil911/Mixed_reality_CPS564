using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Service
{
    interface IProjectAccessClient
    {
        void Initialize(string environmentId, string projectId, CancellationToken cancellationToken);

        Task<List<AccessControlStatement>> GetAsync();
        Task UpsertAsync(IReadOnlyList<AccessControlStatement> authoringStatements);
        Task DeleteAsync(IReadOnlyList<AccessControlStatement> authoringStatements);
    }
}
