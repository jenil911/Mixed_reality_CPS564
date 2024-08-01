using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

class LeaderboardsDeployment : MonoBehaviour
{
    [SerializeField]
    Button m_ButtonAdd;
    [SerializeField]
    Button m_ButtonLog;
    [SerializeField]
    TMP_InputField m_InputScore;

    async void Start()
    {
        ToggleButtons(false);

        m_ButtonAdd.onClick.AddListener(async() => await AddScore_Async());
        m_ButtonLog.onClick.AddListener(async() => await LogScore_Async());

        await InitializeServices();
        await SignInAnonymously();

        ToggleButtons(true);
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

    async Task AddScore_Async()
    {
        ToggleButtons(false);

        try
        {
            var score = double.Parse(m_InputScore.text);
            var result = await LeaderboardsService.Instance.AddPlayerScoreAsync("Sample_Leaderboard", score);

            if (Math.Abs(score - result.Score) > double.Epsilon)
            {
                Debug.Log($"Attempted to add score {score}, but the current score of {result.Score} is better.");
            }
            else
            {
                Debug.Log($"Added score {result.Score} to the leaderboard.");
            }
        }
        finally
        {
            ToggleButtons(true);
        }
    }

    async Task LogScore_Async()
    {
        ToggleButtons(false);

        try
        {
            var result = await LeaderboardsService.Instance.GetPlayerScoreAsync("Sample_Leaderboard");
            Debug.Log($"Score: {result.Score}");
        }
        finally
        {
            ToggleButtons(true);
        }
    }

    void ToggleButtons(bool toggle)
    {
        m_ButtonAdd.interactable = toggle;
        m_ButtonLog.interactable = toggle;
    }
}
