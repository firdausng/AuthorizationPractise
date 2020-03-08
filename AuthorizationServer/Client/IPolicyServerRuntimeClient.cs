using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthorizationServer.Client
{
    public interface IPolicyServerRuntimeClient
    {
        /// <summary>
        /// Evaluates the policy for a given user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        Task<PolicyResult> EvaluateAsync(PolicyRequestDto user);

        /// <summary>
        /// Determines whether the user has a permission.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="permission">The permission.</param>
        /// <returns></returns>
        Task<bool> HasPermissionAsync(PolicyRequestDto user, string permission);

        /// <summary>
        /// Determines whether the user is in a role.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        Task<bool> IsInRoleAsync(PolicyRequestDto user, string role);
    }

    public class PolicyRequestDto
    {
        public Guid TenantId { get; set; }
        public string Subject { get; set; }
        public IEnumerable<string> RoleClaims { get; set; }
    }
}
