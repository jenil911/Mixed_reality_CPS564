namespace Unity.Services.Deployment.Editor.Configuration
{
    interface IProjectPreferences
    {
        string Identifier { get; set; }
        bool GetBool(string key);
        string GetString(string key);
        int GetInt(string key);
        float GetFloat(string key);

        void SetBool(string key, bool value);
        void SetString(string key, string value);
        void SetInt(string key, int value);
        void SetFloat(string key, float value);
    }
}
