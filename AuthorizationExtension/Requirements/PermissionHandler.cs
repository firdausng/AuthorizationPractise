using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthorizationExtension.Requirements
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
            if (await _client.HasPermissionAsync(context.User, requirement.Name))
            {
                context.Succeed(requirement);
            }
        }
    }
}
