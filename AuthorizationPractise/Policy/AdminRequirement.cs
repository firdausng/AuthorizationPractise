using AuthorizationServer.AspNetCore;
using AuthorizationServer.Client;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationPractise.Policy
{
    public class AdminRequirement : IAuthorizationRequirement
    {
    }

    public class AdminRequirementHandler : AuthorizationHandler<AdminRequirement>
    {
        private readonly IPolicyServerRuntimeClient _client;

        public AdminRequirementHandler(IPolicyServerRuntimeClient client)
        {
            _client = client;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            var user = context.User; 
            var dto = user.Convert2PolicyRequest();
            if (await _client.HasPermissionAsync(dto, "AdminAll"))
            {
                context.Succeed(requirement);
            }
        }
    }
}
