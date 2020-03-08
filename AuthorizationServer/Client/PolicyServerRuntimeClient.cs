using AuthorizationServer.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthorizationServer.Client
{
    public class PolicyServerRuntimeClient : IPolicyServerRuntimeClient
    {
        private readonly AuthorizationDbContext context;
        private readonly Guid tenantId;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyServerRuntimeClient"/> class.
        /// </summary>
        /// <param name="policy">The policy.</param>
        public PolicyServerRuntimeClient(AuthorizationDbContext context, Guid TenantId)
        {
            this.context = context;
            tenantId = TenantId;
        }

        /// <summary>
        /// Determines whether the user is in a role.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        public async Task<bool> IsInRoleAsync(PolicyRequestDto user, string role)
        {
            var policy = await EvaluateAsync(user);
            return policy.Roles.Contains(role);
        }

        /// <summary>
        /// Determines whether the user has a permission.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="permission">The permission.</param>
        /// <returns></returns>
        public async Task<bool> HasPermissionAsync(PolicyRequestDto user, string permission)
        {
            var policy = await EvaluateAsync(user);
            return policy.Permissions.Contains(permission);
        }

        /// <summary>
        /// Evaluates the policy for a given user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">user</exception>
        public async Task<PolicyResult> EvaluateAsync(PolicyRequestDto user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            //var policy = await context.Policies.SingleAsync(p => p.TenantId.Equals(tenantId));

            var rolesQuery = context.Roles.Where(r => r.TenantId.Equals(tenantId));
            var sub = user.Subject;
            if (!String.IsNullOrWhiteSpace(sub))
            {
                rolesQuery = rolesQuery
                    .Include(r => r.Subjects)
                    .ThenInclude(r => r.Value.Equals(Guid.Parse(sub)));
            }

            var rolesFromDb = await rolesQuery
                .Select(x => x.Name)
                .ToArrayAsync();

            var roles = user.RoleClaims;

            var permissions = await context.Permissions
                .Where(r => r.TenantId.Equals(tenantId))
                .Include(p => p.Roles)
                .Where(x => x.Roles.Any(r => roles.Any(rr => roles.Contains(rr))))
                .Select(x => x.Name)
                .ToListAsync()
                //.Where(p => p.Roles.Contains(p))
                ;

            var result = new PolicyResult()
            {
                Roles = rolesFromDb.Distinct(),
                Permissions = permissions.Distinct()
            };


            return result;
        }
    }
}
