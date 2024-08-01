using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class CloudCodeScriptsHello : MonoBehaviour
{
    [SerializeField]
    Button m_Button;
    [SerializeField]
    TMP_InputField m_InputField;

    public async void Start()
    {
        m_Button.interactable = false;
        m_Button.onClick.AddListener(async() => await CallModuleEndpoint());

        await InitializeServices();
        await SignInAnonymously();

        m_Button.interactable = true;
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

    async Task CallModuleEndpoint()
    {
        m_Button.interactable = false;

        try
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("name", m_InputField.text);
            var result = await CloudCodeService.Instance.CallEndpointAsync<string>("HelloScript", arguments);
            Debug.Log(result);
        }
        finally
        {
            m_Button.interactable = true;
        }
    }
}
