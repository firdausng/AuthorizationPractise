using AuthorizationServer.Client;
using AuthorizationServer.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the policy server client.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        //public static PolicyServerBuilder AddPolicyServerClient(this IServiceCollection services, Action<PolicyServerOptions> options)
        //{
        //    services.Configure(options);
        //    services.AddHttpClient<IPolicyServerRuntimeClient, PolicyServerHttpClient>();
            
        //    //services.AddSingleton<PolicyServerOptions>();

        //    return new PolicyServerBuilder(services);
        //}

        public static PolicyServerBuilder AddPolicyServerClient(this IServiceCollection services, PolicyServerOptions options)
        {
            services.AddSingleton(options);
            services.AddHttpClient<IPolicyServerRuntimeClient, PolicyServerHttpClient>();

            //services.AddSingleton<PolicyServerOptions>();

            return new PolicyServerBuilder(services);
        }
    }

}
