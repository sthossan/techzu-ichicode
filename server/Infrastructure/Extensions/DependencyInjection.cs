using Server.Application.Interfaces.Core;
using Server.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using Server.Infrastructure.Persistence.Data;
using AutoMapper;

namespace Server.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Database Context
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    builder => builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 2. Generic Repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());

            try
            {
                var applicationAssembly = Assembly.Load("Server.Application");
                var serviceTypes = applicationAssembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service") && t.GetInterfaces().Any(i => i.Name == $"I{t.Name}"));

                foreach (var type in serviceTypes)
                {
                    var interfaceType = type.GetInterfaces().FirstOrDefault(i => i.Name == $"I{type.Name}");
                    if (interfaceType != null)
                    {
                        services.AddScoped(interfaceType, type);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dynamic service registration failed: {ex.Message}");
            }
            return services;
        }
    }

}