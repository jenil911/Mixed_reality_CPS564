using Unity.Services.Core.Editor;
using Unity.Services.Core.Editor.Environments;
using Unity.Services.Deployment.Editor.Analytics;
using Unity.Services.Deployment.Editor.Commands;
using Unity.Services.Deployment.Editor.Configuration;
using Unity.Services.Deployment.Editor.DeploymentDefinitions;
using Unity.Services.Deployment.Editor.Environments;
using Unity.Services.Deployment.Editor.Interface;
using Unity.Services.Deployment.Editor.Interface.UI.Components;
using Unity.Services.Deployment.Editor.Interface.UI.Serialization;
using Unity.Services.Deployment.Editor.IO;
using Unity.Services.Deployment.Editor.JsonUtils;
using Unity.Services.Deployment.Editor.PlayMode;
using Unity.Services.Deployment.Editor.Shared.Analytics;
using Unity.Services.Deployment.Editor.Shared.Assets;
using Unity.Services.Deployment.Editor.Shared.DependencyInversion;
using Unity.Services.Deployment.Editor.Shared.UI;
using Unity.Services.Deployment.Editor.State;
using Unity.Services.Deployment.Editor.Tracking;
using Unity.Services.Deployment.Editor.Validation;
using Unity.Services.DeploymentApi.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.Deployment.Editor
{
    class DeploymentServices : AbstractRuntimeServices<DeploymentServices>
    {
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            using (new AnalyticsTimer(duration => StaticAnalytics.SendInitializeTiming(nameof(DeploymentServices), duration)))
            {
                Instance.Initialize(new ServiceCollection());

                // IEnvironment provider from deployment.api is now obsolete
                // until it is removed, use the new proxy
#pragma warning disable 0612, 0618
                Deployments.Instance.EnvironmentProvider = Instance.GetService<IEnvironmentProvider>();
#pragma warning restore 0612, 0618
#if !UNITY_2023_2_OR_NEWER
                StaticAnalytics.RegisterEvents();
#endif
            }
        }

        public DeploymentServices()
        {
        }

        internal void RegisterInternal(ServiceCollection collection)
        {
            Register(collection);
        }

        public override void Register(ServiceCollection collection)
        {
            collection.Register(_ => Debug.unityLogger);

            collection.RegisterStartupSingleton(Factories.Default<IEditorDeploymentDefinitionService, EditorDeploymentDefinitionService>);

            collection.Register(Factories.Default<IEditorEvents, EditorEvents>);
            collection.Register(Factories.Default<IPlayModeInterrupt, PlayModeInterrupt>);
            collection.RegisterStartupSingleton(Factories.Default<DeployOnPlay>);

            collection.Register(Factories.Default<IDeploymentWindowAnalytics, DeploymentWindowAnalytics>);
            collection.Register(Factories.Default<ICommonAnalytics, CommonAnalytics>);
#if UNITY_2023_2_OR_NEWER
            collection.Register(Factories.Default<ICommonAnalyticProvider, CommonAnalyticProvider>);
#endif
            collection.Register(Factories.Default<IDeployOnPlayAnalytics, DeployOnPlayAnalytics>);
            collection.Register(Factories.Default<IProjectPreferences, ProjectPreferences>);
            collection.Register(Factories.Default<IDeploymentAnalytics, DeploymentAnalytics>);
            collection.RegisterSingleton(Factories.Default<IDeploymentSettings, DeploymentSettings>);

            collection.Register(Factories.Default<IAccessTokens, AccessTokens>);
            collection.Register(Factories.Default<IDeploymentViewModel, DeploymentViewModel>);
            collection.Register(_ => Deployments.Instance.DeploymentProviders);
            collection.Register(_ => Deployments.Instance);
            collection.Register(Factories.Default<IDeploymentWindowStateProvider, DeploymentWindowStateProvider>);
            collection.Register(Factories.Default<IDeploymentItemTracker, DeploymentItemTracker>);
            collection.Register(Factories.Default<INotifications, Notifications>);
            collection.Register(_ => EnvironmentsApi.Instance);

            // IEnvironment provider from deployment.api is now obsolete
            // until it is removed, use the new proxy
            #pragma warning disable
            collection.Register(Factories.Default<IEnvironmentProvider, EnvironmentProxyService>);
            #pragma warning restore

            collection.RegisterStartupSingleton(Factories.Default<DeploymentItemValidator>);

            collection.RegisterSingleton(Factories.Default<ICommandManager, DeploymentCommandManager>);
            collection.Register(Factories.Default<SelectInProjectWindowCommand>);
            collection.Register(Factories.Default<IKeyboardShortcuts, KeyboardShortcuts>);
            collection.Register(Factories.Default<IKeyboardSelectionLogic, KeyboardSelectionLogic>);
            collection.Register(Factories.Default<ISerializationManager, SerializationManager>);
            collection.Register(Factories.Default<ISerializableComponentFetcher, UiTreeSerializableComponentFetcher>);
            collection.Register(Factories.Default<IJsonConverter, NewtonsoftJsonConverter>);
            collection.Register(Factories.Default<IDeployOnPlayItemRetriever, DeployOnPlayItemRetriever>);

            collection.RegisterStartupSingleton(Factories.Default<ItemStatusTracker>);
            collection.Register(Factories.Default<IFileTracker, FileTracker>);
            collection.Register(Factories.Default<AssetPostprocessorProxy>);

#if UNITY_2023_2_OR_NEWER
            collection.Register(Factories.Default<IAnalyticProvider, AnalyticProvider>);
#endif
        }
    }
}
