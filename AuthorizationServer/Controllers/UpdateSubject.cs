using System;

namespace AuthorizationServer.Controllers
{
    public class UpdateSubject
    {
        public Guid TenantId { get; set; }
        public Guid Value { get; set; }
        public Guid RoleId { get; set; }
    }
}
