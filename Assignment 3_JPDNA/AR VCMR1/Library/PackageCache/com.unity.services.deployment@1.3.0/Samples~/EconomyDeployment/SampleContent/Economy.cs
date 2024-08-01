// NOTE: You need to deploy the MYCURRENCY.ecc currency file before being able to run this sample,
// to do so, open the deployment window and deploy the MYCURRENCY.ecc file.
// See README.md for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;

namespace Sample.Deployment.EconomySample
{
    public class Economy : MonoBehaviour
    {
        public string CurrencyId = "MYCURRENCY";
        CurrencyDefinition currency;

        private async void Awake()
        {
            // Economy needs to be initialized and then the user must sign in.
            await InitializeServices();
            await SignInAnonymously();

            // Cache the Economy configuration
            await EconomyService.Instance.Configuration.SyncConfigurationAsync();
            UpdateCurrencyData();
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

        public async void UpdateCurrencyData()
        {
            try
            {
                await EconomyService.Instance.Configuration.SyncConfigurationAsync();
                currency = EconomyService.Instance.Configuration.GetCurrency(CurrencyId);
                Debug.Log($"Currency name: {currency.Name}");
                Debug.Log($"Currency id: {currency.Id}");
                Debug.Log($"Currency initial balance: {currency.Initial}");
                Debug.Log($"Currency max balance: {currency.Max}");
                Debug.Log($"Currency last modified: {currency.Modified.Date}");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async void IncreaseBalance()
        {
            try
            {
                PlayerBalance newBalance = await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(CurrencyId, 1);
                Debug.Log($"New balance for {newBalance.CurrencyId} is {newBalance.Balance}.");
            }
            catch (EconomyRateLimitedException e)
            {
                Debug.LogError($"Rate limit reached - Retry after {e.RetryAfter} seconds.");
            }
            catch (EconomyException e)
            {
                if (e.Reason == EconomyExceptionReason.UnprocessableTransaction)
                {
                    Debug.LogError($"Error while increasing balance: balance maximum reached, please click the reset balance button.");
                    return;
                }

                Debug.LogException(e);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async void ResetBalance()
        {
            try
            {
                PlayerBalance newBalance = await EconomyService.Instance.PlayerBalances.SetBalanceAsync(CurrencyId, currency.Initial);
                Debug.Log($"Balance for {newBalance.CurrencyId} was reset.");
            }
            catch (EconomyRateLimitedException e)
            {
                Debug.LogError($"Rate limit reached - Retry after {e.RetryAfter} seconds.");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
