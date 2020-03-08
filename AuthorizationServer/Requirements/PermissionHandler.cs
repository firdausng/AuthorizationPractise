using AuthorizationServer.AspNetCore;
using AuthorizationServer.Client;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer.Requirements
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPolicyServerRuntimeClient _client;

        public PermissionHandler(IPolicyServerRuntimeClient client)
        {
            _client = client;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var dto = context.User.Convert2PolicyRequest();
                if (await _client.HasPermissionAsync(dto, requirement.Name))
                {
                    context.Succeed(requirement);
                }
            }
            
        }
    }
}
