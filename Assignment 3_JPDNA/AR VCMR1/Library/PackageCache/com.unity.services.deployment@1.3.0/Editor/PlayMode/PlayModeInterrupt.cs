using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Deployment.Editor.Configuration;
using Unity.Services.Deployment.Editor.Shared.EditorUtils;
using UnityEditor;
using UnityEngine;
using DeploymentWindowUi = Unity.Services.Deployment.Editor.Interface.UI.DeploymentWindow;
using Logger = Unity.Services.Deployment.Editor.Shared.Logging.Logger;

namespace Unity.Services.Deployment.Editor.PlayMode
{
    sealed class PlayModeInterrupt : IPlayModeInterrupt, IDisposable
    {
        const string k_BannerMessage = "An error occurred during deployment.";
        const string k_ErrorMessage = "An error occurred during deployment. The state of your server files is unknown and playing could lead to unexpected behavior. To continue anyway, disable the “Deploy on Play” setting from the Deployment Window.";
        readonly List<Func<Task>> m_Interrupts = new List<Func<Task>>();

        internal bool DesiredPlayMode { get; set; } = true;

        readonly IDeploymentSettings m_DeploymentSettings;
        readonly IEditorEvents m_EditorEvents;
        bool m_InterruptExecuting;

        public PlayModeInterrupt(
            IDeploymentSettings deploymentSettings,
            IEditorEvents editorEvents)
        {
            m_DeploymentSettings = deploymentSettings;
            m_EditorEvents = editorEvents;
            m_DeploymentSettings.PropertyChanged += OnDeploymentSettingsPropertyChanged;
            m_EditorEvents.PlayModeStateChanged += PlayModeStateChanged;
        }

        //This is added to make sure that this class does not create unnecessary issues on Play
        void OnDeploymentSettingsPropertyChanged(object obj, PropertyChangedEventArgs args)
        {
            if (args.PropertyName
                is nameof(IDeploymentSettings.BlockPlaymodeOnFailure)
                or nameof(IDeploymentSettings.ShouldDeployOnPlay))
            {
                if (m_DeploymentSettings.ShouldDeployOnPlay)
                {
                    m_EditorEvents.PlayModeStateChanged += PlayModeStateChanged;
                }
                else
                {
                    m_EditorEvents.PlayModeStateChanged -= PlayModeStateChanged;
                }
            }
        }

        public void OnPlay(params Func<Task>[] actions)
        {
            m_Interrupts.Clear();
            m_Interrupts.AddRange(actions);
        }

        public void Dispose()
        {
            m_EditorEvents.PlayModeStateChanged -= PlayModeStateChanged;
        }

        internal void PlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (!m_DeploymentSettings.ShouldDeployOnPlay)
            {
                return;
            }

            if (stateChange == PlayModeStateChange.ExitingEditMode && !m_InterruptExecuting)
            {
                m_InterruptExecuting = true;
                Sync.RunNextUpdateOnMain(async() => await InterruptPlayMode());
            }
        }

        async Task InterruptPlayMode()
        {
            EditorApplication.isPlaying = false;
            var failed = false;

            try
            {
                await Task.WhenAll(m_Interrupts.Select(i => i.Invoke()));
            }
            catch (Exception e)
            {
                failed = true;
                Logger.LogError(k_ErrorMessage);
                Logger.LogException(e);

                if (EditorWindow.HasOpenInstances<DeploymentWindowUi>())
                {
                    var window = EditorWindow.GetWindow<DeploymentWindowUi>();
                    window.ShowNotification(new GUIContent(k_BannerMessage));
                }
            }
            finally
            {
                if (!failed || !m_DeploymentSettings.BlockPlaymodeOnFailure)
                {
                    EditorApplication.isPlaying = DesiredPlayMode;
                }

                m_InterruptExecuting = false;
            }
        }
    }
}
