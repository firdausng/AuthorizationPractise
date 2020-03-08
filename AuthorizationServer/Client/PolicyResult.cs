using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizationServer.Client
{
    public class PolicyResult
    {
        /// <summary>
        /// Gets the roles.
        /// </summary>
        /// <value>
        /// The roles.
        /// </value>
        public IEnumerable<string> Roles { get; set; }

        /// <summary>
        /// Gets the permissions.
        /// </summary>
        /// <value>
        /// The permissions.
        /// </value>
        public IEnumerable<string> Permissions { get; set; }
    }
}
