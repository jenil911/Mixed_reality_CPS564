using System.Collections.Generic;
using Unity.Services.Deployment.Editor.Configuration;
using Unity.Services.Deployment.Editor.JsonUtils;
using Unity.Services.Deployment.Editor.Shared.EditorUtils;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Serialization
{
    class SerializationManager : ISerializationManager
    {
        const string k_SerializationKey = "DeploymentWindow_Serialization";

        readonly IProjectPreferences m_Preferences;
        readonly ISerializableComponentFetcher m_SerializableComponentFetcher;
        readonly IJsonConverter m_JsonConverter;

        VisualElement m_Control;
        State m_State = State.Idle;

        public SerializationManager(
            IProjectPreferences projectPreferences,
            ISerializableComponentFetcher serializableComponentFetcher,
            IJsonConverter jsonConverter)
        {
            m_Preferences = projectPreferences;
            m_SerializableComponentFetcher = serializableComponentFetcher;
            m_JsonConverter = jsonConverter;
        }

        public void Bind(VisualElement control)
        {
            m_Control = control;
        }

        public void Unbind()
        {
            m_Control = null;
        }

        public void ApplySerialization()
        {
            if (m_Control == null) return;

            var serializableComponents = m_SerializableComponentFetcher.GetSerializableComponents(m_Control);
            foreach (var sc in serializableComponents)
            {
                sc.ValueChanged -= TriggerSave;
            }

            var payload = GetSavedSerialization();
            if (payload != null)
            {
                foreach (var sc in serializableComponents)
                {
                    if (sc.SerializationKey != null
                        && payload.ContainsKey(sc.SerializationKey))
                    {
                        sc.ApplySerialization(payload[sc.SerializationKey]);
                    }
                }
            }

            foreach (var sc in serializableComponents)
            {
                sc.ValueChanged += TriggerSave;
            }
        }

        public Dictionary<string, object> GetSavedSerialization()
        {
            return m_JsonConverter.DeserializeObject<Dictionary<string, object>>(m_Preferences.GetString(k_SerializationKey));
        }

        void TriggerSave()
        {
            if (m_State == State.Idle)
            {
                Sync.RunNextUpdateOnMain(() =>
                {
                    Save();
                    m_State = State.Idle;
                });
                m_State = State.Pending;
            }
        }

        internal void Save()
        {
            var serializableComponents = m_SerializableComponentFetcher.GetSerializableComponents(m_Control);
            var dictionary = new Dictionary<string, object>();
            serializableComponents.ForEach(sc =>
            {
                if (sc.SerializationKey != null)
                {
                    dictionary.Add(
                        sc.SerializationKey,
                        sc.SerializationValue);
                }
            });
            var payload = m_JsonConverter.SerializeObject(dictionary);
            m_Preferences.SetString(k_SerializationKey, payload);
        }

        enum State
        {
            Idle,
            Pending
        }
    }
}
