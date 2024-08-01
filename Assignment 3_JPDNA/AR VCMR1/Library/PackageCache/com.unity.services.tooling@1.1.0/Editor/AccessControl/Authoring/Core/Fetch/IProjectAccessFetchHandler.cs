using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Results;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Fetch
{
    interface IProjectAccessFetchHandler
    {
        public Task<FetchResult> FetchAsync(
            string rootDirectory,
            IReadOnlyList<IProjectAccessFile> files,
            bool dryRun = false,
            bool reconcile = false,
            CancellationToken token = default);
    }
}
