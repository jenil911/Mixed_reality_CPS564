using System.Collections.Generic;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model
{
    class ProjectAccessParser : IProjectAccessParser
    {
        public List<AccessControlStatement> ParseFile(ProjectAccessFileContent content, IProjectAccessFile file)
        {
            var authoringStatements = new List<AccessControlStatement>();
            foreach (var statement in content.Statements)
            {
                statement.Path = file.Path;
                statement.Name = statement.Sid;
                authoringStatements.Add(statement);
            }

            return authoringStatements;
        }
    }
}
