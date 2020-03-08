using AuthorizationServer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthorizationServer.AspNetCore
{
    public static class ClaimsPrincipalExtension
    {

        public static PolicyRequestDto Convert2PolicyRequest(this ClaimsPrincipal user)
        {
            //var tenantIdClaim = user.Identity.FirstOrDefault(c => c.Type == "tenant_id");
            var identity = user.Identity as ClaimsIdentity;
            var userClaims = identity.Claims;
            var tenantIdClaim = userClaims.FirstOrDefault(c => c.Type == "tenant_id");

            var dto = new PolicyRequestDto
            {
                Subject = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value,
                //RoleClaims = user.FindAll("role").Select(x => x.Value),
                RoleClaims = user.FindAll("role").Select(x => x.Value),
                TenantId = Guid.Parse(tenantIdClaim.Value)
                //TenantId = tenantId
            };

            return dto;
        }
    }
}
