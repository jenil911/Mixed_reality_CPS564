using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.IO
{
    interface IAccessControlResourcesLoader
    {
        Task LoadFileAsync(IProjectAccessFile projectAccessFile, CancellationToken cancellationToken);
    }
}
