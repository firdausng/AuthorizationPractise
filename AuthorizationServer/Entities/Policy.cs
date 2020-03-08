using System.Collections.Generic;

namespace AuthorizationServer.Entities
{
    public class Policy: BaseEntity
    {
        public List<Role> Roles { get; internal set; } = new List<Role>();
        public List<Permission> Permissions { get; internal set; } = new List<Permission>();
    }
}
