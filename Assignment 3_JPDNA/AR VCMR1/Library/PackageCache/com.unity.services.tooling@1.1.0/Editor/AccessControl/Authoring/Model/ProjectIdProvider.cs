using UnityEditor;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Model
{
    class ProjectIdProvider : IProjectIdProvider
    {
        public string ProjectId => CloudProjectSettings.projectId;
    }
}
