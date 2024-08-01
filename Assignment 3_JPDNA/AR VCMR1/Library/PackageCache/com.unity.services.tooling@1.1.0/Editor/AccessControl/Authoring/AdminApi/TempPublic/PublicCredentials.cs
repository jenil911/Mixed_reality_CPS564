using System;
using System.IO;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.AdminApi.TempPublic
{
    class PublicCredentials
    {
        public static string Get()
        {
            var credentialsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "UnityServices/credentials");

            // just temp code, we wont sanitize
            if (!File.Exists(credentialsPath))
            {
                return default;
            }

            return File.ReadAllText(credentialsPath);
        }
    }
}
