using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthorizationServer.Client
{
    public class PolicyServerHttpClient : IPolicyServerRuntimeClient
    {
        private readonly PolicyServerOptions options;
        private readonly ILogger<PolicyServerHttpClient> logger;

        public PolicyServerHttpClient(HttpClient client, PolicyServerOptions options, ILogger<PolicyServerHttpClient> logger)
        {
            this.options = options;
            this.logger = logger;
            client.BaseAddress = new Uri(this.options.BaseUrl);
            // GitHub API versioning
            // client.DefaultRequestHeaders.Add("Accept","application/vnd.github.v3+json");

            Client = client;

        }

        public HttpClient Client { get; }

        public async Task<PolicyResult> EvaluateAsync(PolicyRequestDto user)
        {
            logger.LogInformation("Evaluating user to get policy");
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(Client.BaseAddress, "/policy/evaluate"),
                Content = new StringContent(
                JsonConvert.SerializeObject(user),
                    Encoding.UTF8, "application/json")
            };

            var response = await Client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("we got error");
            }

            var token = JsonConvert.DeserializeObject<PolicyResult>(await response.Content.ReadAsStringAsync());

            return token;
        }

        public async Task<bool> HasPermissionAsync(PolicyRequestDto user, string permission)
        {
            var policy = await EvaluateAsync(user);
            return policy.Permissions.Contains(permission);
        }

        public async Task<bool> IsInRoleAsync(PolicyRequestDto user, string role)
        {
            var policy = await EvaluateAsync(user);
            return policy.Roles.Contains(role);
        }
    }


}