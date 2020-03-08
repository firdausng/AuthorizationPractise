using AuthorizationServer.Entities;
using AuthorizationServer.Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IDistributedCache cache;
        private readonly AuthorizationDbContext context;

        public RoleController(ILogger<RoleController> logger,
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
            var role = await context.Roles
                .Where(r => r.TenantId.Equals(tenantId))
                .Include(r => r.Subjects)
                .Include(r => r.IdentityRoles)
                .ToListAsync();
            return Ok(role);
        }

        [HttpPost("{roleId}/subject")]
        public async Task<IActionResult> UpdateAsync(UpdateSubject updateSubject, Guid roleId)
        {
            var role = await context.Roles
                .Include(r => r.Subjects)
                .FirstAsync(r => r.Id.Equals(roleId));

            var subject = new Subject
            {
                Value = updateSubject.Value,
                TenantId = updateSubject.TenantId,
            };

            role.Subjects.Add(subject);
            //context.Roles.Update(roleFromDb);

            context.SaveChanges();

            await cache.RefreshAsync(updateSubject.Value.ToString());
            return Ok(role);
        }
    }

    public class UpdateRole
    {
    }
}