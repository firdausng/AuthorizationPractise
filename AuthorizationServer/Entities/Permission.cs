using AuthorizationServer.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthorizationServer.Entities
{
    public class Permission: BaseEntity
    {
        public string Name { get; set; }
        public List<PermissionRole> Roles { get; set; } = new List<PermissionRole>();
    }
}
