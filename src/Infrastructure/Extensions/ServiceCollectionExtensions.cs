using Application.Abstractions;
using Domain.SharedKernel;
using Infrastructure.Repositories;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Get the connection string from appsettings.json
        var connectionString = configuration.GetConnectionString("Database");

        // Configure and register Marten
        services.AddMarten(options => { options.Connection(connectionString!); })
            .UseLightweightSessions(); // Use lightweight sessions for better performance

        // Register our repository implementation
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();

        return services;
    }
}