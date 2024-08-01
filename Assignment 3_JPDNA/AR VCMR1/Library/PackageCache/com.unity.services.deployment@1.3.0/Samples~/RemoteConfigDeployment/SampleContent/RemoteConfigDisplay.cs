using System.Globalization;
using Sample.Deployment.RemoteConfigSample;
using TMPro;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.UI;

public class RemoteConfigDisplay : MonoBehaviour
{
    [SerializeReference] RemoteConfig m_RemoteConfig;

    [SerializeField] TextMeshProUGUI m_StringValue;
    [SerializeField] TextMeshProUGUI m_IntValue;
    [SerializeField] Toggle m_BoolValue;
    [SerializeField] TextMeshProUGUI m_LongValue;
    [SerializeField] TextMeshProUGUI m_FloatValue;
    [SerializeField] TextMeshProUGUI m_JsonValue;

    void Awake()
    {
        if (!m_RemoteConfig)
        {
            Debug.LogError("Remote Config is null");
            return;
        }

        m_RemoteConfig.OnRemoteConfigUpdated += DisplayRemoteConfig;
    }

    public void GetConfig()
    {
        var fetchConfigsAsync = m_RemoteConfig.FetchConfigsAsync();
    }

    void DisplayRemoteConfig(RuntimeConfig config)
    {
        m_StringValue.text = config.GetString("string_key");
        m_IntValue.text = config.GetInt("int_key").ToString();
        m_BoolValue.isOn = config.GetBool("bool_key");
        m_LongValue.text = config.GetLong("long_key").ToString();
        m_FloatValue.text = config.GetFloat("float_key").ToString(CultureInfo.InvariantCulture);
        m_JsonValue.text = config.GetJson("json_key");
    }
}
