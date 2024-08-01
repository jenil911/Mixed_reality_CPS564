using System.Collections.Generic;
using System.Linq;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model
{
    class ProjectAccessFileContent
    {
        public readonly IReadOnlyList<AccessControlStatement> Statements;

        public ProjectAccessFileContent()
        {
            Statements = new List<AccessControlStatement>();
        }
        public ProjectAccessFileContent(IReadOnlyList<AccessControlStatement> statements)
        {
            Statements = new List<AccessControlStatement>(statements);
        }

        public List<AccessControlStatement> ToAuthoringStatements(IProjectAccessFile file, IProjectAccessParser parser)
        {
            return parser.ParseFile(this, file).ToList();
        }
    }
}
