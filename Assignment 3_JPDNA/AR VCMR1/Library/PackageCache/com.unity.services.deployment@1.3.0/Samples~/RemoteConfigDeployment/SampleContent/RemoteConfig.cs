// NOTE: You need to deploy the new-remote-config.rc file before being able to run this sample,
// to do so, open the deployment window and deploy the new-remote-config.rc file.
// See README.md for more details.

using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.RemoteConfig;

namespace Sample.Deployment.RemoteConfigSample
{
    public class RemoteConfig : MonoBehaviour
    {
        public RuntimeConfig CashedConfig { get; private set; }

        public Action<RuntimeConfig> OnRemoteConfigUpdated;
        struct userAttributes
        {
            // Optionally declare variables for any custom user attributes:
            public bool expansionFlag;
        }

        struct appAttributes
        {
            // Optionally declare variables for any custom app attributes:
            public int level;
            public int score;
            public string appVersion;
        }

        async void Awake()
        {
            // Remote Config needs to be initialized and then the user must sign in.
            await InitializeServices();
            await SignInAnonymously();

            RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;

            await FetchConfigsAsync();
        }

        public async Task FetchConfigsAsync()
        {
            await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());
        }

        void ApplyRemoteConfig(ConfigResponse obj)
        {
            CashedConfig = RemoteConfigService.Instance.appConfig;

            OnRemoteConfigUpdated?.Invoke(CashedConfig);
        }

        static async Task InitializeServices()
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                await UnityServices.InitializeAsync();
            }
        }

        static async Task SignInAnonymously()
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
    }
}
