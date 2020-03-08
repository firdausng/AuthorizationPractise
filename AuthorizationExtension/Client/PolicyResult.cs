using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizationExtension.Client
{
    public class PolicyResult
    {
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> Permissions { get; set; }
    }
}
