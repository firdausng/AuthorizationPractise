using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer.Entities
{
    public abstract class BaseEntity
    {
        public Guid TenantId { get; set; }
        public Guid Id { get; set; }
    }
}
