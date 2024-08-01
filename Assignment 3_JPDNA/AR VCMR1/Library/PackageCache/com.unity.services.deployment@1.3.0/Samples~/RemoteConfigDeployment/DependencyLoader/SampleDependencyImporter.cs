using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageManagerClient = UnityEditor.PackageManager.Client;

class SampleDependencyImporter
{
    const string k_SampleDependencyFilename = "sample-dependencies.json";

    [InitializeOnLoadMethod]
    static void Initialization()
    {
        RunSampleDependencyImporter();
    }

    static void RunSampleDependencyImporter(
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
    {
        var scriptsDirectory = Directory.GetParent(sourceFilePath);
        var dependenciesFilepath = Path.Combine(scriptsDirectory!.Parent!.FullName, k_SampleDependencyFilename);
        InstallPackageDependencies(dependenciesFilepath);
        DeleteFolder(scriptsDirectory!.FullName);
    }

    /// <summary>
    /// Install packages from Sample Configuration file
    /// </summary>
    /// <param name="sampleConfigFilepath">filepath to Sample Configuration file</param>
    static void InstallPackageDependencies(string sampleConfigFilepath)
    {
        if (File.Exists(sampleConfigFilepath))
        {
            var configurationText = File.ReadAllText(sampleConfigFilepath);
            var packagesToInstall = GetSampleDependenciesToInstall(configurationText);
            InstallMultiplePackages(packagesToInstall);
            AssetDatabase.Refresh();
        }
    }

    static List<string> GetSampleDependenciesToInstall(string configurationText)
    {
        var sampleConfig = JsonUtility.FromJson<JsonListWrapper<string>>(configurationText).dependencies;

        var packageList = PackageManagerClient.List(true);
        while (!packageList.IsCompleted);

        List<string> packagesToInstall = new List<string>();
        foreach (var packageId in sampleConfig)
        {
            if (HasHigherVersion(packageId, packageList))
            {
                packagesToInstall.Add(packageId);
            }
        }

        return packagesToInstall;
    }

    static void InstallMultiplePackages(List<string> packageIds)
    {
        if (packageIds.Any())
        {
            var packageAdd = PackageManagerClient.AddAndRemove(packageIds.ToArray(), null);
            while (!packageAdd.IsCompleted) {}

            foreach (var packageId in packageIds)
            {
                Debug.Log($"Installed package {packageId}.");
            }
        }
    }

    /// <summary>
    /// we implement a very simple version comparator, we don't want to implement semver
    /// so we truncate everything after the "-" in the version string
    /// </summary>
    /// <param name="semver"></param>
    /// <returns></returns>
    static Version GetTruncatedVersion(string semver)
    {
        var semverSuffixPos = semver.IndexOf("-");
        return new Version(semverSuffixPos > -1 ? semver.Substring(0, semverSuffixPos) : semver);
    }

    /// <summary>
    /// Checks if the packageId is a higher version or not present in current manifest
    /// </summary>
    /// <param name="packageId"></param>
    /// <param name="packageList"></param>
    /// <returns>Returns true if new package has higher version or is does not exist in manifest</returns>
    static bool HasHigherVersion(string packageId, ListRequest packageList)
    {
        var separatorPos = packageId.IndexOf("@");
        var newPackageName = packageId.Substring(0, separatorPos);
        var newPackageVersion = packageId.Substring(separatorPos + 1);

        var curPackageInfo = packageList.Result.FirstOrDefault(p => p.packageId.StartsWith(newPackageName));
        // Checks if package is already present and has higher version
        if (curPackageInfo != null)
        {
            var curVersion = GetTruncatedVersion(curPackageInfo.version);
            var newVersion = GetTruncatedVersion(newPackageVersion);
            if (newVersion.CompareTo(curVersion) <= 0)
            {
                return false;
            }
        }

        return true;
    }

    static void DeleteFolder(string directoryPath)
    {
        File.Delete(directoryPath + ".meta");
        Directory.Delete(directoryPath, recursive: true);
        AssetDatabase.Refresh();
    }

    [System.Serializable]
    public class JsonListWrapper<T>
    {
        public List<T> dependencies;
        public JsonListWrapper(List<T> dependencies) => this.dependencies = dependencies;
    }
}
