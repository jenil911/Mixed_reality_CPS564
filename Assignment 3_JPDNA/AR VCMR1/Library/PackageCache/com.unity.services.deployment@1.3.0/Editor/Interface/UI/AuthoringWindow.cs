using System;
using UnityEditor;

namespace Unity.Services.Deployment.Editor.Interface.UI
{
    abstract class AuthoringWindow : EditorWindow
    {
        [NonSerialized]
        protected bool m_Initialized;

        protected virtual void CreateGUI()
        {
            // This method should be overriden by test windows
            // so as to not use the real RuntimeServices
            DeploymentServices.Instance.InitializeInstance(this);

            if (!m_Initialized)
            {
                LoadGui();
            }

            m_Initialized = true;
        }

        protected internal abstract void LoadGui();
    }
}
