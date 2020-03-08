using AuthorizationPractise.Infra;
using AuthorizationPractise.Infra.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;

namespace AuthorizationPractise
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<ApplicationDbContext>();
                var logger = services.GetService<ILogger<Program>>();
                context.Database.Migrate();

                var mainTenant = context.Tenants.Where(t => t.Name.Equals("Testnt")).FirstOrDefaultAsync().Result;
                if (mainTenant == null)
                {
                    logger.LogInformation("cannot find Testnt tenant");
                    mainTenant = new Tenant
                    {
                        Name = "Testnt",

                    };
                    context.Tenants.Add(mainTenant);
                    context.SaveChanges();
                    logger.LogInformation("Testnt tenant created");
                }
                else
                {
                    logger.LogInformation("Testnt tenant created");
                }


                var secondTenant = context.Tenants.Where(t => t.Name.Equals("second")).FirstOrDefaultAsync().Result;
                if (secondTenant == null)
                {
                    logger.LogInformation("cannot find second tenant");
                    secondTenant = new Tenant
                    {
                        Name = "second",

                    };
                    context.Tenants.Add(secondTenant);
                    context.SaveChanges();
                    logger.LogInformation("second tenant created");
                }
                else
                {
                    logger.LogInformation("second tenant created");
                }

                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var alice = userManager.FindByNameAsync("alice").Result;
                if (alice == null)
                {
                    logger.LogInformation("Creating Alice");
                    alice = new ApplicationUser
                    {
                        UserName = "alice",
                        Email = "AliceSmith@email.com",
                        EmailConfirmed = true,
                        TenantId = mainTenant.Id
                    };
                    var result = userManager.CreateAsync(alice, "Password@01").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userManager.AddClaimsAsync(alice, new Claim[]{
                        new Claim("tenant_id", alice.TenantId.ToString()),
                    }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    logger.LogInformation("alice created");
                }

                var daus = userManager.FindByNameAsync("daus").Result;
                if (daus == null)
                {
                    logger.LogInformation("Creating daus");
                    daus = new ApplicationUser
                    {
                        UserName = "daus",
                        Email = "daus@email.com",
                        EmailConfirmed = true,
                        TenantId = secondTenant.Id
                    };
                    var result = userManager.CreateAsync(daus, "Password@01").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userManager.AddClaimsAsync(daus, new Claim[]{
                        new Claim("tenant_id", daus.TenantId.ToString()),
                    }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    logger.LogInformation("daus created");
                }

                var gee = userManager.FindByNameAsync("gee").Result;
                if (gee == null)
                {
                    logger.LogInformation("Creating gee");
                    gee = new ApplicationUser
                    {
                        UserName = "gee",
                        Email = "gee@email.com",
                        EmailConfirmed = true,
                        TenantId = mainTenant.Id
                    };
                    var result = userManager.CreateAsync(gee, "Password@01").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    result = userManager.AddClaimsAsync(gee, new Claim[]{
                        new Claim("tenant_id", gee.TenantId.ToString()),
                    }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    logger.LogInformation("gee created");
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
