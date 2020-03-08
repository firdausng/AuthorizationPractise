using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationPractise.Infra.Data
{
    public class ApplicationUser: IdentityUser
    {
        public Guid TenantId { get; set; }
    }
}
