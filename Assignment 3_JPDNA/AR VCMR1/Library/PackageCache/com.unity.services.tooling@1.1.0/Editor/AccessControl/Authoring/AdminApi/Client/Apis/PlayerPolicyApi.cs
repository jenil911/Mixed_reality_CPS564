//-----------------------------------------------------------------------------
// <auto-generated>
//     This file was generated by the C# SDK Code Generator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------------


using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Models;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Http;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.PlayerPolicy;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Apis.PlayerPolicy
{
    /// <summary>
    /// Interface for the PlayerPolicyApiClient
    /// </summary>
    internal interface IPlayerPolicyApiClient
    {
            /// <summary>
            /// Async Operation.
            /// Delete policy statement(s).
            /// </summary>
            /// <param name="request">Request object for DeletePlayerPolicyStatements.</param>
            /// <param name="operationConfiguration">Configuration for DeletePlayerPolicyStatements.</param>
            /// <returns>Task for a Response object containing status code, headers.</returns>
            /// <exception cref="Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Http.HttpException">An exception containing the HttpClientResponse with headers, response code, and string of error.</exception>
            Task<Response> DeletePlayerPolicyStatementsAsync(Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.PlayerPolicy.DeletePlayerPolicyStatementsRequest request, Configuration operationConfiguration = null);

            /// <summary>
            /// Async Operation.
            /// Get all player based policies in project.
            /// </summary>
            /// <param name="request">Request object for GetAllPlayerPolicies.</param>
            /// <param name="operationConfiguration">Configuration for GetAllPlayerPolicies.</param>
            /// <returns>Task for a Response object containing status code, headers, and Models.PlayerPolicies object.</returns>
            /// <exception cref="Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Http.HttpException">An exception containing the HttpClientResponse with headers, response code, and string of error.</exception>
            Task<Response<Models.PlayerPolicies>> GetAllPlayerPoliciesAsync(Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.PlayerPolicy.GetAllPlayerPoliciesRequest request, Configuration operationConfiguration = null);

            /// <summary>
            /// Async Operation.
            /// Get a player based policy.
            /// </summary>
            /// <param name="request">Request object for GetPlayerPolicy.</param>
            /// <param name="operationConfiguration">Configuration for GetPlayerPolicy.</param>
            /// <returns>Task for a Response object containing status code, headers, and Models.PlayerPolicy object.</returns>
            /// <exception cref="Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Http.HttpException">An exception containing the HttpClientResponse with headers, response code, and string of error.</exception>
            Task<Response<Models.PlayerPolicy>> GetPlayerPolicyAsync(Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.PlayerPolicy.GetPlayerPolicyRequest request, Configuration operationConfiguration = null);

            /// <summary>
            /// Async Operation.
            /// Create or update a player based policy.
            /// </summary>
            /// <param name="request">Request object for UpsertPlayerPolicy.</param>
            /// <param name="operationConfiguration">Configuration for UpsertPlayerPolicy.</param>
            /// <returns>Task for a Response object containing status code, headers.</returns>
            /// <exception cref="Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Http.HttpException">An exception containing the HttpClientResponse with headers, response code, and string of error.</exception>
            Task<Response> UpsertPlayerPolicyAsync(Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.PlayerPolicy.UpsertPlayerPolicyRequest request, Configuration operationConfiguration = null);

    }

    ///<inheritdoc cref="IPlayerPolicyApiClient"/>
    internal class PlayerPolicyApiClient : BaseApiClient, IPlayerPolicyApiClient
    {
        private const int _baseTimeout = 10;
        private Configuration _configuration;
        /// <summary>
        /// Accessor for the client configuration object. This returns a merge
        /// between the current configuration and the global configuration to
        /// ensure the correct combination of headers and a base path (if it is
        /// set) are returned.
        /// </summary>
        public Configuration Configuration
        {
            get {
                // We return a merge between the current configuration and the
                // global configuration to ensure we have the correct
                // combination of headers and a base path (if it is set).
                Configuration globalConfiguration = new Configuration("https://services.api.unity.com", 10, 4, null);
                return Configuration.MergeConfigurations(_configuration, globalConfiguration);
            }
            set { _configuration = value; }
        }

        /// <summary>
        /// PlayerPolicyApiClient Constructor.
        /// </summary>
        /// <param name="httpClient">The HttpClient for PlayerPolicyApiClient.</param>
        /// <param name="configuration"> PlayerPolicyApiClient Configuration object.</param>
        public PlayerPolicyApiClient(IHttpClient httpClient,
            Configuration configuration = null) : base(httpClient)
        {
            // We don't need to worry about the configuration being null at
            // this stage, we will check this in the accessor.
            _configuration = configuration;

            
        }


        /// <summary>
        /// Async Operation.
        /// Delete policy statement(s).
        /// </summary>
        /// <param name="request">Request object for DeletePlayerPolicyStatements.</param>
        /// <param name="operationConfiguration">Configuration for DeletePlayerPolicyStatements.</param>
        /// <returns>Task for a Response object containing status code, headers.</returns>
        /// <exception cref="Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Http.HttpException">An exception containing the HttpClientResponse with headers, response code, and string of error.</exception>
        public async Task<Response> DeletePlayerPolicyStatementsAsync(Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.PlayerPolicy.DeletePlayerPolicyStatementsRequest request,
            Configuration operationConfiguration = null)
        {
            var statusCodeToTypeMap = new Dictionary<string, System.Type>() { {"204",  null },{"400", typeof(Models.ValidationError)   },{"401", typeof(Models.AuthenticationError)   },{"403", typeof(Models.AuthorizationError)   },{"404", typeof(Models.NotFoundError)   },{"429", typeof(Models.TooManyRequestsError)   },{"500", typeof(Models.InternalServerError)   },{"503", typeof(Models.ServiceUnavailableError)   } };

            // Merge the operation/request level configuration with the client level configuration.
            var finalConfiguration = Configuration.MergeConfigurations(operationConfiguration, Configuration);

            var response = await HttpClient.MakeRequestAsync("POST",
                request.ConstructUrl(finalConfiguration.BasePath),
                request.ConstructBody(),
                request.ConstructHeaders(finalConfiguration),
                finalConfiguration.RequestTimeout ?? _baseTimeout,
                finalConfiguration.RetryPolicyConfiguration,
                finalConfiguration.StatusCodePolicyConfiguration);

            ResponseHandler.HandleAsyncResponse(response, statusCodeToTypeMap);
            return new Response(response);
        }


        /// <summary>
        /// Async Operation.
        /// Get all player based policies in project.
        /// </summary>
        /// <param name="request">Request object for GetAllPlayerPolicies.</param>
        /// <param name="operationConfiguration">Configuration for GetAllPlayerPolicies.</param>
        /// <returns>Task for a Response object containing status code, headers, and Models.PlayerPolicies object.</returns>
        /// <exception cref="Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Http.HttpException">An exception containing the HttpClientResponse with headers, response code, and string of error.</exception>
        public async Task<Response<Models.PlayerPolicies>> GetAllPlayerPoliciesAsync(Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.PlayerPolicy.GetAllPlayerPoliciesRequest request,
            Configuration operationConfiguration = null)
        {
            var statusCodeToTypeMap = new Dictionary<string, System.Type>() { {"200", typeof(Models.PlayerPolicies)   },{"401", typeof(Models.AuthenticationError)   },{"403", typeof(Models.AuthorizationError)   },{"404", typeof(Models.NotFoundError)   },{"429", typeof(Models.TooManyRequestsError)   },{"500", typeof(Models.InternalServerError)   },{"503", typeof(Models.ServiceUnavailableError)   } };

            // Merge the operation/request level configuration with the client level configuration.
            var finalConfiguration = Configuration.MergeConfigurations(operationConfiguration, Configuration);

            var response = await HttpClient.MakeRequestAsync("GET",
                request.ConstructUrl(finalConfiguration.BasePath),
                request.ConstructBody(),
                request.ConstructHeaders(finalConfiguration),
                finalConfiguration.RequestTimeout ?? _baseTimeout,
                finalConfiguration.RetryPolicyConfiguration,
                finalConfiguration.StatusCodePolicyConfiguration);

            var handledResponse = ResponseHandler.HandleAsyncResponse<Models.PlayerPolicies>(response, statusCodeToTypeMap);
            return new Response<Models.PlayerPolicies>(response, handledResponse);
        }


        /// <summary>
        /// Async Operation.
        /// Get a player based policy.
        /// </summary>
        /// <param name="request">Request object for GetPlayerPolicy.</param>
        /// <param name="operationConfiguration">Configuration for GetPlayerPolicy.</param>
        /// <returns>Task for a Response object containing status code, headers, and Models.PlayerPolicy object.</returns>
        /// <exception cref="Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Http.HttpException">An exception containing the HttpClientResponse with headers, response code, and string of error.</exception>
        public async Task<Response<Models.PlayerPolicy>> GetPlayerPolicyAsync(Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.PlayerPolicy.GetPlayerPolicyRequest request,
            Configuration operationConfiguration = null)
        {
            var statusCodeToTypeMap = new Dictionary<string, System.Type>() { {"200", typeof(Models.PlayerPolicy)   },{"401", typeof(Models.AuthenticationError)   },{"403", typeof(Models.AuthorizationError)   },{"404", typeof(Models.NotFoundError)   },{"429", typeof(Models.TooManyRequestsError)   },{"500", typeof(Models.InternalServerError)   },{"503", typeof(Models.ServiceUnavailableError)   } };

            // Merge the operation/request level configuration with the client level configuration.
            var finalConfiguration = Configuration.MergeConfigurations(operationConfiguration, Configuration);

            var response = await HttpClient.MakeRequestAsync("GET",
                request.ConstructUrl(finalConfiguration.BasePath),
                request.ConstructBody(),
                request.ConstructHeaders(finalConfiguration),
                finalConfiguration.RequestTimeout ?? _baseTimeout,
                finalConfiguration.RetryPolicyConfiguration,
                finalConfiguration.StatusCodePolicyConfiguration);

            var handledResponse = ResponseHandler.HandleAsyncResponse<Models.PlayerPolicy>(response, statusCodeToTypeMap);
            return new Response<Models.PlayerPolicy>(response, handledResponse);
        }


        /// <summary>
        /// Async Operation.
        /// Create or update a player based policy.
        /// </summary>
        /// <param name="request">Request object for UpsertPlayerPolicy.</param>
        /// <param name="operationConfiguration">Configuration for UpsertPlayerPolicy.</param>
        /// <returns>Task for a Response object containing status code, headers.</returns>
        /// <exception cref="Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Http.HttpException">An exception containing the HttpClientResponse with headers, response code, and string of error.</exception>
        public async Task<Response> UpsertPlayerPolicyAsync(Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.PlayerPolicy.UpsertPlayerPolicyRequest request,
            Configuration operationConfiguration = null)
        {
            var statusCodeToTypeMap = new Dictionary<string, System.Type>() { {"204",  null },{"400", typeof(Models.ValidationError)   },{"401", typeof(Models.AuthenticationError)   },{"403", typeof(Models.AuthorizationError)   },{"404", typeof(Models.NotFoundError)   },{"429", typeof(Models.TooManyRequestsError)   },{"500", typeof(Models.InternalServerError)   },{"503", typeof(Models.ServiceUnavailableError)   } };

            // Merge the operation/request level configuration with the client level configuration.
            var finalConfiguration = Configuration.MergeConfigurations(operationConfiguration, Configuration);

            var response = await HttpClient.MakeRequestAsync("PATCH",
                request.ConstructUrl(finalConfiguration.BasePath),
                request.ConstructBody(),
                request.ConstructHeaders(finalConfiguration),
                finalConfiguration.RequestTimeout ?? _baseTimeout,
                finalConfiguration.RetryPolicyConfiguration,
                finalConfiguration.StatusCodePolicyConfiguration);

            ResponseHandler.HandleAsyncResponse(response, statusCodeToTypeMap);
            return new Response(response);
        }

    }
}