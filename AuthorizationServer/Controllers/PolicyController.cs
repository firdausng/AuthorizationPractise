using AuthorizationServer.Client;
using AuthorizationServer.Entities;
using AuthorizationServer.Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
//using AuthorizationServer.Extensions;

namespace AuthorizationServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PolicyController : ControllerBase
    {
        private readonly ILogger<PolicyController> _logger;
        private readonly IDistributedCache cache;
        private readonly AuthorizationDbContext context;

        public PolicyController(
            ILogger<PolicyController> logger,
            IDistributedCache cache,
            AuthorizationDbContext context)
        {
            _logger = logger;
            this.cache = cache;
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(Guid tenantId)
        {
            var policy = await context.Policies
                .Include(p => p.Permissions)
                .Include(p => p.Roles)
                .ThenInclude(r => r.Subjects)
                .SingleAsync(p => p.TenantId.Equals(tenantId));
            return Ok(policy);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Policy updatePolicy)
        {
            var policy = context.Policies
                .Update(updatePolicy);
            await context.SaveChangesAsync();
            return Ok(policy);
        }


        [HttpPost("Evaluate")]
        public async Task<PolicyResult> PostEvaluateAsync(PolicyRequestDto policyRequest)
        {
            if (policyRequest == null) throw new ArgumentNullException(nameof(policyRequest));

            var cacheResponseJson = await cache.GetStringAsync(policyRequest.Subject);
            if (cacheResponseJson != null)
            {
                return JsonConvert.DeserializeObject<PolicyResult>(cacheResponseJson);
            }
            
            //var policy = await context.Policies.SingleAsync(p => p.TenantId.Equals(tenantId));

            var rolesQuery = context.Roles.Where(r => r.TenantId.Equals(policyRequest.TenantId));
            var sub = policyRequest.Subject;
            if (!String.IsNullOrWhiteSpace(sub))
            {
                rolesQuery = rolesQuery
                    .Include(r => r.Subjects)
                    .Where(x => x.Subjects.Any(r => r.Value.Equals(Guid.Parse(sub))))
                    //.ThenInclude(r => r.Id.Equals(Guid.Parse(sub)))
                    ;
            }

            var rolesFromDb = await rolesQuery
                .Select(x => x.Name)
                .ToArrayAsync();

            //var roles = policyRequest.RoleClaims;

            var permissions = await context.Permissions
                .Where(r => r.TenantId.Equals(policyRequest.TenantId))
                .Where(r => r.Roles.Any(a => rolesFromDb.Contains(a.Role.Name)))
                .Select(x => x.Name)
                .ToListAsync();


            var result = new PolicyResult()
            {
                Roles = rolesFromDb.Distinct(),
                Permissions = permissions.Distinct()
            };
            var json = JsonConvert.SerializeObject(result);

            await cache.SetStringAsync(policyRequest.Subject, json, new DistributedCacheEntryOptions { SlidingExpiration= TimeSpan.FromMinutes(30) });

            return result;
        }
    }

    public class UpdatePolicy
    {
        public Guid TenantId { get; set; }
    }
}
