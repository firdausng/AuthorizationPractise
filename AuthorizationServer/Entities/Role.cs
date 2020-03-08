using System;
using System.Collections.Generic;

namespace AuthorizationServer.Entities
{
    public class Role: BaseEntity
    {
        public string Name { get; set; }
        public List<Subject> Subjects { get; internal set; } = new List<Subject>();
        public List<IdentityRole> IdentityRoles { get; internal set; } = new List<IdentityRole>();
        public List<PermissionRole> Permissions { get; set; } = new List<PermissionRole>();
    }

    public class IdentityRole : BaseEntity
    {
        public Guid RoleId { get; set; }
        public string Value { get; set; }
    }

    public class Subject : BaseEntity
    {
        public Guid RoleId { get; set; }
        public Guid Value { get; set; }
    }
}
