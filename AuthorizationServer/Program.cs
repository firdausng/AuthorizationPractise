using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Entities;
using AuthorizationServer.Infra.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuthorizationServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<AuthorizationDbContext>();
                var logger = services.GetService<ILogger<Program>>();

                var tenantId = Guid.Parse("7c2b0438-5a33-4023-ad11-c0fcd3075346");
                var tenant = context.Policies.FirstOrDefault(p => p.TenantId.Equals(tenantId));

                if (tenant == null)
                {

                    logger.LogInformation("Creating AdminAll permission");
                    var adminAllPermission = new Permission
                    {
                        TenantId = tenantId,
                        Name = "AdminAll",
                    };
                    logger.LogInformation("Creating admin role");
                    var adminRole = new Role
                    {
                        TenantId = tenantId,
                        Name = "Admin",
                        Subjects = new List<Subject>
                        {
                            new Subject{TenantId= tenantId, Value=Guid.Parse("d613f1b7-e0c5-459e-af4f-7c4681378085")},
                            //new Subject{TenantId= tenantId, Value=Guid.Parse("e3cc45c3-abb7-4a58-9267-15ea763706ee")},
                        }
                        //Subjects = new string[] { "142a000d-29b7-47a9-b34f-9b7a9acbe4af", "7ee3c1a8-fdc6-49fe-a065-3572860e3f26" }
                    };
                    adminAllPermission.Roles.Add(new PermissionRole
                    {
                        Role = adminRole
                    });

                    logger.LogInformation("Creating tenant 1 policies");
                    tenant = new Policy
                    {
                        TenantId = tenantId, 
                    };
                    tenant.Roles.Add(adminRole);
                    tenant.Permissions.Add(adminAllPermission);
                    var a = context.Policies.AddAsync(tenant).Result;
                    var result = context.SaveChangesAsync().Result;
                    //if (!result.Succeeded)
                    //{
                    //    throw new Exception(result.Errors.First().Description);
                    //}
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
