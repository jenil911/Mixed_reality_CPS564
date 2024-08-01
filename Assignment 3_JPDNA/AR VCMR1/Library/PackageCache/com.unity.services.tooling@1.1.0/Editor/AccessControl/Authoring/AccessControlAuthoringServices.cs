using System.Collections.ObjectModel;
using Unity.Services.Core.Editor;
using Unity.Services.Core.Editor.Environments;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.AdminApi;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Analytics;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Client;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Apis.ProjectPolicy;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.ErrorMitigation;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Http;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Deploy;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.IO;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Json;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Service;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Validations;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Deployment;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.IO;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Model;
using Unity.Services.Tooling.Editor.Shared.Analytics;
using Unity.Services.Tooling.Editor.Shared.DependencyInversion;
using UnityEditor;
using UnityEngine;
using ILogger = Unity.Services.Tooling.Editor.AccessControl.Authoring.Logging.ILogger;
using Logger = Unity.Services.Tooling.Editor.AccessControl.Authoring.Logging.Logger;
using static Unity.Services.Tooling.Editor.Shared.DependencyInversion.Factories;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring
{
    class AccessControlAuthoringServices : AbstractRuntimeServices<AccessControlAuthoringServices>
    {
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            Instance.Initialize(new ServiceCollection());
            var deploymentItemProvider = Instance.GetService<DeploymentProvider>();
            Deployments.Instance.DeploymentProviders.Add(deploymentItemProvider);
        }

        public override void Register(ServiceCollection collection)
        {
            collection.RegisterSingleton(Default<ObservableCollection<IDeploymentItem>, ObservableAccessControlAssets>);
            collection.Register(Default<IProjectIdProvider, ProjectIdProvider>);
            collection.Register(_ => new Configuration(null, null, null, null));
            collection.Register(Default<IRetryPolicyProvider, RetryPolicyProvider>);
            collection.Register(Default<IHttpClient, HttpClient>);
            collection.Register(Default<IProjectPolicyApiClient, ProjectPolicyApiClient>);
            collection.Register(c => (ObservableAccessControlAssets)c.GetService(typeof(ObservableCollection<IDeploymentItem>)));
            collection.Register(_ => Debug.unityLogger);
            collection.Register(Default<ICommonAnalytics, CommonAnalytics>);
#if UNITY_2023_2_OR_NEWER
            collection.Register(Default<ICommonAnalyticProvider, CommonAnalyticProvider>);
#endif
            collection.Register(Default<IAccessControlEditorAnalytics, AccessControlEditorAnalytics>);
            collection.Register(Default<DeployCommand>);
            collection.Register(Default<IProjectAccessConfigValidator, ProjectAccessConfigValidator>);
            collection.Register(Default<IProjectAccessMerger, ProjectAccessMerger>);
            collection.Register(Default<IProjectAccessDeploymentHandler, ProjectAccessDeploymentHandler>);
            collection.Register(Default<IProjectAccessClient, AccessControlClient>);
            collection.Register(Default<IAccessTokens, AccessTokens>);
            collection.Register(_ => EnvironmentsApi.Instance);
            collection.RegisterStartupSingleton(Default<DeploymentProvider, AccessControlDeploymentProvider>);
            collection.Register(Default<ILogger, Logger>);
            collection.Register(Default<IJsonConverter, JsonConverter>);
            collection.Register(Default<IFileSystem, FileSystem>);
            collection.Register(Default<IProjectAccessParser, ProjectAccessParser>);
            collection.Register(Default<IAccessControlResourcesLoader, AccessControlResourcesLoader>);
        }
    }
}
