using System.Collections.Generic;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model
{
    interface IProjectAccessParser
    {
        List<AccessControlStatement> ParseFile(ProjectAccessFileContent content, IProjectAccessFile file);
    }
}
