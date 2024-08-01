using UnityEditor;

namespace Unity.Services.Deployment.Editor.Configuration
{
    class ProjectPreferences : IProjectPreferences
    {
        public string Identifier { get; set; }

        public ProjectPreferences()
        {
            Identifier = PlayerSettings.productGUID.ToString();
        }

        public bool GetBool(string key)
        {
            return EditorPrefs.GetBool($"{Identifier}_{key}");
        }

        public string GetString(string key)
        {
            return EditorPrefs.GetString($"{Identifier}_{key}");
        }

        public int GetInt(string key)
        {
            return EditorPrefs.GetInt($"{Identifier}_{key}");
        }

        public float GetFloat(string key)
        {
            return EditorPrefs.GetFloat($"{Identifier}_{key}");
        }

        public void SetBool(string key, bool value)
        {
            EditorPrefs.SetBool($"{Identifier}_{key}", value);
        }

        public void SetString(string key, string value)
        {
            EditorPrefs.SetString($"{Identifier}_{key}", value);
        }

        public void SetInt(string key, int value)
        {
            EditorPrefs.SetInt($"{Identifier}_{key}", value);
        }

        public void SetFloat(string key, float value)
        {
            EditorPrefs.SetFloat($"{Identifier}_{key}", value);
        }
    }
}
