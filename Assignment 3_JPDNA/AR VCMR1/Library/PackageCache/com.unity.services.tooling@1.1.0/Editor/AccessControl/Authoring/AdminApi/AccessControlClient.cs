#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Core.Editor;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Service;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Logging;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.AdminApi.TempPublic;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Client;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Apis.ProjectPolicy;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Http;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.Models;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Client.ProjectPolicy;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.ErrorHandling;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Model;
using Unity.Services.Tooling.Editor.Shared.Clients;
using Unity.Services.Core.Editor.Environments;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.AdminApi
{
    class AccessControlClient : IProjectAccessClient
    {
        readonly IAccessTokens m_TokenProvider;
        readonly IProjectPolicyApiClient m_Client;

        readonly IEnvironmentsApi m_EnvironmentProvider;
        readonly IProjectIdProvider m_ProjectIdProvider;

        const int k_MaxStatementsPerUpsertQuery = 500;

        public AccessControlClient(
            IAccessTokens tokenProviders,
            IProjectPolicyApiClient client,
            IEnvironmentsApi environmentProvider,
            IProjectIdProvider projectIdProvider)
        {
            m_TokenProvider = tokenProviders;
            m_Client = client;
            m_EnvironmentProvider = environmentProvider;
            m_ProjectIdProvider = projectIdProvider;
        }

        public void Initialize(string environmentId, string projectId, CancellationToken cancellationToken)
        {
        }

        async Task UpdateTokenAsync()
        {
            // In the editor, we should refresh the token, or else in a long enough
            // Unity session, the token will expire and calls will fail
            var client = m_Client as ProjectPolicyApiClient;
            if (client == null)
            {
                return;
            }

            var token = await m_TokenProvider.GetServicesGatewayTokenAsync();
            var headers = new AdminApiHeaders<AccessControlClient>(token);
            client.Configuration = new Configuration(
                null,
                null,
                null,
                headers.ToDictionary());
        }

        public async Task<List<AccessControlStatement>> GetAsync()
        {
            await UpdateTokenAsync();
            var request = new GetPolicyRequest(
                m_ProjectIdProvider.ProjectId,
                m_EnvironmentProvider.ActiveEnvironmentId.ToString()
            );
            var res = await WrapException(m_Client.GetPolicyAsync(request));
            return res.Result.Statements.Select(statement => new AccessControlStatement()
            {
                Resource = statement.Resource,
                Action = statement.Action,
                Effect = statement.Effect,
                Sid = statement.Sid,
                Principal = statement.Principal,
                ExpiresAt = statement.ExpiresAt,
                Version = statement.Version,
            }).ToList();
        }

        public async Task UpsertAsync(IReadOnlyList<AccessControlStatement> authoringStatements)
        {
            await UpdateTokenAsync();
            var chunks = ChunkIterator(authoringStatements, k_MaxStatementsPerUpsertQuery);
            foreach (var chunk in chunks)
            {
                var request = new UpsertPolicyRequest(
                    m_ProjectIdProvider.ProjectId,
                    m_EnvironmentProvider.ActiveEnvironmentId.ToString(),
                    new Policy(chunk.Select(statement => new ProjectStatement(
                        statement.Sid,
                        statement.Action,
                        statement.Effect,
                        statement.Principal,
                        statement.Resource,
                        statement.ExpiresAt,
                        statement.Version
                        )).ToList())
                );

                // NOTE: we lose the atomicity of the UpsertAsync operation here, if operation#1+ fails, then there's
                // no obvious way of reverting the changes of the preceding operations other than tracking all the changes
                // that have been done so far, and piling them in an undo List<Task> variable.
                await WrapException(m_Client.UpsertPolicyAsync(request));
            }
        }

        public async Task DeleteAsync(IReadOnlyList<AccessControlStatement> authoringStatements)
        {
            await UpdateTokenAsync();
            var request = new DeletePolicyStatementsRequest(
                m_ProjectIdProvider.ProjectId,
                m_EnvironmentProvider.ActiveEnvironmentId.ToString(),
                new DeleteOptions(authoringStatements.Select(statement => statement.Sid).ToList())
            );
            
            await WrapException(m_Client.DeletePolicyStatementsAsync(request));
        }

        static IEnumerable<TSource[]> ChunkIterator<TSource>(IEnumerable<TSource> source, int size)
        {
            using IEnumerator<TSource> e = source.GetEnumerator();
            while (e.MoveNext())
            {
                TSource[] chunk = new TSource[size];
                chunk[0] = e.Current;

                int i = 1;
                for (; i < chunk.Length && e.MoveNext(); i++)
                {
                    chunk[i] = e.Current;
                }

                if (i == chunk.Length)
                {
                    yield return chunk;
                }
                else
                {
                    Array.Resize(ref chunk, i);
                    yield return chunk;
                    yield break;
                }
            }
        }

        static async Task<T> WrapException<T>(Task<T> query)
        {
            T res;
            try
            {
                res = await query;
            }
            catch (HttpException<AuthenticationError> e)
            {
                throw new AdminApiException(e.ActualError.Title, e.ActualError.Detail, e);
            }
            catch (HttpException<AuthorizationError> e)
            {
                throw new AdminApiException(e.ActualError.Title, e.ActualError.Detail, e);
            }
            catch (HttpException<NotFoundError> e)
            {
                throw new AdminApiException(e.ActualError.Title, e.ActualError.Detail, e);
            }
            catch (HttpException<TooManyRequestsError> e)
            {
                throw new AdminApiException(e.ActualError.Title, e.ActualError.Detail, e);
            }
            catch (HttpException<InternalServerError> e)
            {
                throw new AdminApiException(e.ActualError.Title, "The server encountered en error.", e);
            }
            catch (HttpException<ServiceUnavailableError> e)
            {
                throw new AdminApiException(e.ActualError.Title, "The service is unavailable.", e);
            }
            catch (HttpException<ValidationError> e)
            {
                throw new AdminApiException(e.ActualError.Title, e.ActualError.Detail, e);
            }
            catch (HttpException e)
            {
                throw new AdminApiException("Unknown Server Error", "An unexpected error happened during the processing of this query.", e);
            }

            return res;
        }
    }
}
