// WARNING: Auto generated code. Modifications will be lost!
// Original source 'com.unity.services.shared' @0.0.12.

using System.Collections.Generic;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.AdminApi.TempPublic
{
    class PublicAdminApiHeaders<T>
    {
        readonly string m_PublicToken;

        public PublicAdminApiHeaders(string publicToken)
        {
            m_PublicToken = publicToken;
        }

        public IDictionary<string, string> ToDictionary()
        {
            var packageInfo = ReadPackageInfo();

            return new Dictionary<string, string>
            {
                { "Authorization", $"Basic {m_PublicToken}" },
                { "x-client-id", $"{packageInfo.name}@{packageInfo.version}"}
            };
        }

        static PackageInfo ReadPackageInfo()
        {
            return PackageInfo.FindForAssembly(typeof(T).Assembly);
        }
    }
}
