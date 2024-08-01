//-----------------------------------------------------------------------------
// <auto-generated>
//     This file was generated by the C# SDK Code Generator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Models;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Scheduler;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Http;


namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.PlayerPolicy
{
    internal static class JsonSerialization
    {
        public static byte[] Serialize<T>(T obj)
        {
            return Encoding.UTF8.GetBytes(SerializeToString(obj));
        }

        public static string SerializeToString<T>(T obj)
        {
            return IsolatedJsonConvert.SerializeObject(obj, new JsonSerializerSettings{ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore});
        }
    }

    /// <summary>
    /// PlayerPolicyApiBaseRequest class
    /// </summary>
    [Preserve]
    internal class PlayerPolicyApiBaseRequest
    {
        /// <summary>
        /// Helper function to add a provided key and value to the provided
        /// query params and to escape the values correctly if it is a URL.
        /// </summary>
        /// <param name="queryParams">A `List/<string/>` of the query parameters.</param>
        /// <param name="key">The key to be added.</param>
        /// <param name="value">The value to be added.</param>
        /// <returns>Returns a `List/<string/>` with the `key` and `value` added to the provided `queryParams`.</returns>
        [Preserve]
        public List<string> AddParamsToQueryParams(List<string> queryParams, string key, string value)
        {
            key = UnityWebRequest.EscapeURL(key);
            value = UnityWebRequest.EscapeURL(value);
            queryParams.Add($"{key}={value}");

            return queryParams;
        }

        /// <summary>
        /// Helper function to add a provided key and list of values to the
        /// provided query params and to escape the values correctly if it is a
        /// URL.
        /// </summary>
        /// <param name="queryParams">A `List/<string/>` of the query parameters.</param>
        /// <param name="key">The key to be added.</param>
        /// <param name="values">List of values to be added.</param>
        /// <param name="style">string for defining the style, currently unused.</param>
        /// <param name="explode">True if query params should be escaped and added separately.</param>
        /// <returns>Returns a `List/<string/>`</returns>
        [Preserve]
        public List<string> AddParamsToQueryParams(List<string> queryParams, string key, List<string> values, string style, bool explode)
        {
            if (explode)
            {
                foreach(var value in values)
                {
                    string escapedValue = UnityWebRequest.EscapeURL(value);
                    queryParams.Add($"{UnityWebRequest.EscapeURL(key)}={escapedValue}");
                }
            }
            else
            {
                string paramString = $"{UnityWebRequest.EscapeURL(key)}=";
                foreach(var value in values)
                {
                    paramString += UnityWebRequest.EscapeURL(value) + ",";
                }
                paramString = paramString.Remove(paramString.Length - 1);
                queryParams.Add(paramString);
            }

            return queryParams;
        }

        /// <summary>
        /// Helper function to add a provided map of keys and values, representing a model, to the
        /// provided query params.
        /// </summary>
        /// <param name="queryParams">A `List/<string/>` of the query parameters.</param>
        /// <param name="modelVars">A `Dictionary` representing the vars of the model</param>
        /// <returns>Returns a `List/<string/>`</returns>
        [Preserve]
        public List<string> AddParamsToQueryParams(List<string> queryParams, Dictionary<string, string> modelVars)
        {
            foreach(var key in modelVars.Keys)
            {
                string escapedValue = UnityWebRequest.EscapeURL(modelVars[key]);
                queryParams.Add($"{UnityWebRequest.EscapeURL(key)}={escapedValue}");
            }

            return queryParams;
        }

        /// <summary>
        /// Helper function to add a provided key and value to the provided
        /// query params and to escape the values correctly if it is a URL.
        /// </summary>
        /// <param name="queryParams">A `List/<string/>` of the query parameters.</param>
        /// <param name="key">The key to be added.</param>
        /// <typeparam name="T">The type of the value to be added.</typeparam>
        /// <param name="value">The value to be added.</param>
        /// <returns>Returns a `List/<string/>`</returns>
        [Preserve]
        public List<string> AddParamsToQueryParams<T>(List<string> queryParams, string key, T value)
        {
            if (queryParams == null)
            {
                queryParams = new List<string>();
            }

            key = UnityWebRequest.EscapeURL(key);
            string valueString = UnityWebRequest.EscapeURL(value.ToString());
            queryParams.Add($"{key}={valueString}");
            return queryParams;
        }

        /// <summary>
        /// Constructs a string representing an array path parameter.
        /// </summary>
        /// <param name="pathParam">The list of values to convert to string.</param>
        /// <returns>String representing the param.</returns>
        [Preserve]
        public string GetPathParamString(List<string> pathParam)
        {
            string paramString = "";
            foreach(var value in pathParam)
            {
                paramString += UnityWebRequest.EscapeURL(value) + ",";
            }
            paramString = paramString.Remove(paramString.Length - 1);
            return paramString;
        }

        /// <summary>
        /// Constructs the body of the request based on IO stream.
        /// </summary>
        /// <param name="stream">The IO stream to use.</param>
        /// <returns>Byte array representing the body.</returns>
        public byte[] ConstructBody(System.IO.Stream stream)
        {
            if (stream != null)
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
            return null;
        }

        /// <summary>
        /// Construct the request body based on string value.
        /// </summary>
        /// <param name="s">The input body.</param>
        /// <returns>Byte array representing the body.</returns>
        public byte[] ConstructBody(string s)
        {
            return System.Text.Encoding.UTF8.GetBytes(s);
        }

        /// <summary>
        /// Construct request body based on generic object.
        /// </summary>
        /// <param name="o">The object to use.</param>
        /// <returns>Byte array representing the body.</returns>
        public byte[] ConstructBody(object o)
        {
            return JsonSerialization.Serialize(o);
        }

        /// <summary>
        /// Generate an accept header.
        /// </summary>
        /// <param name="accepts">list of accepts objects.</param>
        /// <returns>The generated accept header.</returns>
        public string GenerateAcceptHeader(string[] accepts)
        {
            if (accepts.Length == 0)
            {
                return null;
            }
            for (int i = 0; i < accepts.Length; ++i)
            {
                if (string.Equals(accepts[i], "application/json", System.StringComparison.OrdinalIgnoreCase))
                {
                    return "application/json";
                }
            }
            return string.Join(", ", accepts);
        }

        private static readonly Regex JsonRegex = new Regex(@"application\/json(;\s)?((charset=utf8|q=[0-1]\.\d)(\s)?)*");

        /// <summary>
        /// Generate Content Type Header.
        /// </summary>
        /// <param name="contentTypes">The content types.</param>
        /// <returns>The Content Type Header.</returns>
        public string GenerateContentTypeHeader(string[] contentTypes)
        {
            if (contentTypes.Length == 0)
            {
                return null;
            }

            for(int i = 0; i < contentTypes.Length; ++i)
            {
                if (!string.IsNullOrWhiteSpace(contentTypes[i]) && JsonRegex.IsMatch(contentTypes[i]))
                {
                    return contentTypes[i];
                }
            }
            return contentTypes[0];
        }

        /// <summary>
        /// Generate multipart form file section.
        /// </summary>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="stream">The file stream to use.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>Returns a multipart form section.</returns>
        public IMultipartFormSection GenerateMultipartFormFileSection(string paramName, System.IO.FileStream stream, string contentType)
        {
            return new MultipartFormFileSection(paramName, ConstructBody(stream), GetFileName(stream.Name), contentType);
        }

        /// <summary>
        /// Generate multipart form file section.
        /// </summary>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="stream">The IO stream to use.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>Returns a multipart form section.</returns>
        public IMultipartFormSection GenerateMultipartFormFileSection(string paramName, System.IO.Stream stream, string contentType)
        {
            return new MultipartFormFileSection(paramName, ConstructBody(stream), Guid.NewGuid().ToString(), contentType);
        }

        private string GetFileName(string filePath)
        {
            return System.IO.Path.GetFileName(filePath);
        }
    }

    /// <summary>
    /// DeletePlayerPolicyStatementsRequest
    /// Delete policy statement(s)
    /// </summary>
    [Preserve]
    internal class DeletePlayerPolicyStatementsRequest : PlayerPolicyApiBaseRequest
    {
        /// <summary>Accessor for projectId </summary>
        [Preserve]
        public string ProjectId { get; }
        /// <summary>Accessor for environmentId </summary>
        [Preserve]
        public string EnvironmentId { get; }
        /// <summary>Accessor for playerId </summary>
        [Preserve]
        public string PlayerId { get; }
        /// <summary>Accessor for deleteOptions </summary>
        [Preserve]
        public Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Models.DeleteOptions DeleteOptions { get; }
        string PathAndQueryParams;

        /// <summary>
        /// DeletePlayerPolicyStatements Request Object.
        /// Delete policy statement(s)
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="environmentId">ID of the environment</param>
        /// <param name="playerId">ID of the player</param>
        /// <param name="deleteOptions">DeleteOptions param</param>
        [Preserve]
        public DeletePlayerPolicyStatementsRequest(string projectId, string environmentId, string playerId, Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Models.DeleteOptions deleteOptions)
        {
            ProjectId = projectId;

            EnvironmentId = environmentId;

            PlayerId = playerId;

            DeleteOptions = deleteOptions;
            PathAndQueryParams = $"/access/resource-policy/v1/projects/{projectId}/environments/{environmentId}/players/{playerId}/resource-policy:delete-statements";


        }

        /// <summary>
        /// Helper function for constructing URL from request base path and
        /// query params.
        /// </summary>
        /// <param name="requestBasePath"></param>
        /// <returns></returns>
        public string ConstructUrl(string requestBasePath)
        {
            return requestBasePath + PathAndQueryParams;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public byte[] ConstructBody()
        {
            return ConstructBody(DeleteOptions);
        }

        /// <summary>
        /// Helper function for constructing the headers.
        /// </summary>
        /// <param name="operationConfiguration">The operation configuration to use.</param>
        /// <returns>A dictionary representing the request headers.</returns>
        public Dictionary<string, string> ConstructHeaders(Configuration operationConfiguration = null)
        {
            var headers = new Dictionary<string, string>();
            // Generator Does not support base username password, unable to add headers for security schema: ServiceAccount
            // Configure HTTP bearer authorization: Bearer
            if(!string.IsNullOrEmpty(operationConfiguration.Bearer))
            {
                headers.Add("authorization", "Bearer " + operationConfiguration.Bearer);
            }

            // Analytics headers
            headers.Add("Unity-Client-Version", Application.unityVersion);
            headers.Add("Unity-Client-Mode", Scheduler.EngineStateHelper.IsPlaying ? "play" : "edit");

            string[] contentTypes = {
                "application/json"
            };

            string[] accepts = {
                "application/problem+json"
            };

            var acceptHeader = GenerateAcceptHeader(accepts);
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                headers.Add("Accept", acceptHeader);
            }
            var httpMethod = "POST";
            var contentTypeHeader = GenerateContentTypeHeader(contentTypes);
            if (!string.IsNullOrEmpty(contentTypeHeader))
            {
                headers.Add("Content-Type", contentTypeHeader);
            }
            else if (httpMethod == "POST" || httpMethod == "PATCH")
            {
                headers.Add("Content-Type", "application/json");
            }


            // We also check if there are headers that are defined as part of
            // the request configuration.
            if (operationConfiguration != null && operationConfiguration.Headers != null)
            {
                foreach (var pair in operationConfiguration.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            return headers;
        }
    }
    /// <summary>
    /// GetAllPlayerPoliciesRequest
    /// Get all player based policies in project
    /// </summary>
    [Preserve]
    internal class GetAllPlayerPoliciesRequest : PlayerPolicyApiBaseRequest
    {
        /// <summary>Accessor for projectId </summary>
        [Preserve]
        public string ProjectId { get; }
        /// <summary>Accessor for environmentId </summary>
        [Preserve]
        public string EnvironmentId { get; }
        /// <summary>Accessor for limit </summary>
        [Preserve]
        public int? Limit { get; }
        /// <summary>Accessor for next </summary>
        [Preserve]
        public string Next { get; }
        string PathAndQueryParams;

        /// <summary>
        /// GetAllPlayerPolicies Request Object.
        /// Get all player based policies in project
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="environmentId">ID of the environment</param>
        /// <param name="limit">max results per page - is ignored if `next` query parameter is provided</param>
        /// <param name="next">pagination token for requesting next page of results</param>
        [Preserve]
        public GetAllPlayerPoliciesRequest(string projectId, string environmentId, int? limit = default(int?), string next = default(string))
        {
            ProjectId = projectId;

            EnvironmentId = environmentId;

            Limit = limit;
            Next = next;
            PathAndQueryParams = $"/access/resource-policy/v1/projects/{projectId}/environments/{environmentId}/players/resource-policy";

            List<string> queryParams = new List<string>();

            var limitStringValue = Limit.ToString();
            queryParams = AddParamsToQueryParams(queryParams, "limit", limitStringValue);
            if(!string.IsNullOrEmpty(Next))
            {
                queryParams = AddParamsToQueryParams(queryParams, "next", Next);
            }
            if (queryParams.Count > 0)
            {
                PathAndQueryParams = $"{PathAndQueryParams}?{string.Join("&", queryParams)}";
            }
        }

        /// <summary>
        /// Helper function for constructing URL from request base path and
        /// query params.
        /// </summary>
        /// <param name="requestBasePath"></param>
        /// <returns></returns>
        public string ConstructUrl(string requestBasePath)
        {
            return requestBasePath + PathAndQueryParams;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public byte[] ConstructBody()
        {
            return null;
        }

        /// <summary>
        /// Helper function for constructing the headers.
        /// </summary>
        /// <param name="operationConfiguration">The operation configuration to use.</param>
        /// <returns>A dictionary representing the request headers.</returns>
        public Dictionary<string, string> ConstructHeaders(Configuration operationConfiguration = null)
        {
            var headers = new Dictionary<string, string>();
            // Generator Does not support base username password, unable to add headers for security schema: ServiceAccount
            // Configure HTTP bearer authorization: Bearer
            if(!string.IsNullOrEmpty(operationConfiguration.Bearer))
            {
                headers.Add("authorization", "Bearer " + operationConfiguration.Bearer);
            }

            // Analytics headers
            headers.Add("Unity-Client-Version", Application.unityVersion);
            headers.Add("Unity-Client-Mode", Scheduler.EngineStateHelper.IsPlaying ? "play" : "edit");

            string[] contentTypes = {
            };

            string[] accepts = {
                "application/json",
                "application/problem+json"
            };

            var acceptHeader = GenerateAcceptHeader(accepts);
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                headers.Add("Accept", acceptHeader);
            }
            var httpMethod = "GET";
            var contentTypeHeader = GenerateContentTypeHeader(contentTypes);
            if (!string.IsNullOrEmpty(contentTypeHeader))
            {
                headers.Add("Content-Type", contentTypeHeader);
            }
            else if (httpMethod == "POST" || httpMethod == "PATCH")
            {
                headers.Add("Content-Type", "application/json");
            }


            // We also check if there are headers that are defined as part of
            // the request configuration.
            if (operationConfiguration != null && operationConfiguration.Headers != null)
            {
                foreach (var pair in operationConfiguration.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            return headers;
        }
    }
    /// <summary>
    /// GetPlayerPolicyRequest
    /// Get a player based policy
    /// </summary>
    [Preserve]
    internal class GetPlayerPolicyRequest : PlayerPolicyApiBaseRequest
    {
        /// <summary>Accessor for projectId </summary>
        [Preserve]
        public string ProjectId { get; }
        /// <summary>Accessor for environmentId </summary>
        [Preserve]
        public string EnvironmentId { get; }
        /// <summary>Accessor for playerId </summary>
        [Preserve]
        public string PlayerId { get; }
        string PathAndQueryParams;

        /// <summary>
        /// GetPlayerPolicy Request Object.
        /// Get a player based policy
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="environmentId">ID of the environment</param>
        /// <param name="playerId">ID of the player</param>
        [Preserve]
        public GetPlayerPolicyRequest(string projectId, string environmentId, string playerId)
        {
            ProjectId = projectId;

            EnvironmentId = environmentId;

            PlayerId = playerId;

            PathAndQueryParams = $"/access/resource-policy/v1/projects/{projectId}/environments/{environmentId}/players/{playerId}/resource-policy";


        }

        /// <summary>
        /// Helper function for constructing URL from request base path and
        /// query params.
        /// </summary>
        /// <param name="requestBasePath"></param>
        /// <returns></returns>
        public string ConstructUrl(string requestBasePath)
        {
            return requestBasePath + PathAndQueryParams;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public byte[] ConstructBody()
        {
            return null;
        }

        /// <summary>
        /// Helper function for constructing the headers.
        /// </summary>
        /// <param name="operationConfiguration">The operation configuration to use.</param>
        /// <returns>A dictionary representing the request headers.</returns>
        public Dictionary<string, string> ConstructHeaders(Configuration operationConfiguration = null)
        {
            var headers = new Dictionary<string, string>();
            // Generator Does not support base username password, unable to add headers for security schema: ServiceAccount
            // Configure HTTP bearer authorization: Bearer
            if(!string.IsNullOrEmpty(operationConfiguration.Bearer))
            {
                headers.Add("authorization", "Bearer " + operationConfiguration.Bearer);
            }

            // Analytics headers
            headers.Add("Unity-Client-Version", Application.unityVersion);
            headers.Add("Unity-Client-Mode", Scheduler.EngineStateHelper.IsPlaying ? "play" : "edit");

            string[] contentTypes = {
            };

            string[] accepts = {
                "application/json",
                "application/problem+json"
            };

            var acceptHeader = GenerateAcceptHeader(accepts);
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                headers.Add("Accept", acceptHeader);
            }
            var httpMethod = "GET";
            var contentTypeHeader = GenerateContentTypeHeader(contentTypes);
            if (!string.IsNullOrEmpty(contentTypeHeader))
            {
                headers.Add("Content-Type", contentTypeHeader);
            }
            else if (httpMethod == "POST" || httpMethod == "PATCH")
            {
                headers.Add("Content-Type", "application/json");
            }


            // We also check if there are headers that are defined as part of
            // the request configuration.
            if (operationConfiguration != null && operationConfiguration.Headers != null)
            {
                foreach (var pair in operationConfiguration.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            return headers;
        }
    }
    /// <summary>
    /// UpsertPlayerPolicyRequest
    /// Create or update a player based policy
    /// </summary>
    [Preserve]
    internal class UpsertPlayerPolicyRequest : PlayerPolicyApiBaseRequest
    {
        /// <summary>Accessor for projectId </summary>
        [Preserve]
        public string ProjectId { get; }
        /// <summary>Accessor for environmentId </summary>
        [Preserve]
        public string EnvironmentId { get; }
        /// <summary>Accessor for playerId </summary>
        [Preserve]
        public string PlayerId { get; }
        /// <summary>Accessor for playerPolicyUpsert </summary>
        [Preserve]
        public Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Models.PlayerPolicyUpsert PlayerPolicyUpsert { get; }
        string PathAndQueryParams;

        /// <summary>
        /// UpsertPlayerPolicy Request Object.
        /// Create or update a player based policy
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="environmentId">ID of the environment</param>
        /// <param name="playerId">ID of the player</param>
        /// <param name="playerPolicyUpsert">PlayerPolicyUpsert param</param>
        [Preserve]
        public UpsertPlayerPolicyRequest(string projectId, string environmentId, string playerId, Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Models.PlayerPolicyUpsert playerPolicyUpsert)
        {
            ProjectId = projectId;

            EnvironmentId = environmentId;

            PlayerId = playerId;

            PlayerPolicyUpsert = playerPolicyUpsert;
            PathAndQueryParams = $"/access/resource-policy/v1/projects/{projectId}/environments/{environmentId}/players/{playerId}/resource-policy";


        }

        /// <summary>
        /// Helper function for constructing URL from request base path and
        /// query params.
        /// </summary>
        /// <param name="requestBasePath"></param>
        /// <returns></returns>
        public string ConstructUrl(string requestBasePath)
        {
            return requestBasePath + PathAndQueryParams;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public byte[] ConstructBody()
        {
            return ConstructBody(PlayerPolicyUpsert);
        }

        /// <summary>
        /// Helper function for constructing the headers.
        /// </summary>
        /// <param name="operationConfiguration">The operation configuration to use.</param>
        /// <returns>A dictionary representing the request headers.</returns>
        public Dictionary<string, string> ConstructHeaders(Configuration operationConfiguration = null)
        {
            var headers = new Dictionary<string, string>();
            // Generator Does not support base username password, unable to add headers for security schema: ServiceAccount
            // Configure HTTP bearer authorization: Bearer
            if(!string.IsNullOrEmpty(operationConfiguration.Bearer))
            {
                headers.Add("authorization", "Bearer " + operationConfiguration.Bearer);
            }

            // Analytics headers
            headers.Add("Unity-Client-Version", Application.unityVersion);
            headers.Add("Unity-Client-Mode", Scheduler.EngineStateHelper.IsPlaying ? "play" : "edit");

            string[] contentTypes = {
                "application/json"
            };

            string[] accepts = {
                "application/problem+json"
            };

            var acceptHeader = GenerateAcceptHeader(accepts);
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                headers.Add("Accept", acceptHeader);
            }
            var httpMethod = "PATCH";
            var contentTypeHeader = GenerateContentTypeHeader(contentTypes);
            if (!string.IsNullOrEmpty(contentTypeHeader))
            {
                headers.Add("Content-Type", contentTypeHeader);
            }
            else if (httpMethod == "POST" || httpMethod == "PATCH")
            {
                headers.Add("Content-Type", "application/json");
            }


            // We also check if there are headers that are defined as part of
            // the request configuration.
            if (operationConfiguration != null && operationConfiguration.Headers != null)
            {
                foreach (var pair in operationConfiguration.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            return headers;
        }
    }
}
