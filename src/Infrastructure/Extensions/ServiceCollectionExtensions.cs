using Application.Abstractions;
using Domain.SharedKernel;
using Infrastructure.Projections;
using Infrastructure.Repositories;
using Infrastructure.Serialization;
using JasperFx.Events.Projections;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Get the connection string from appsettings.json
        var connectionString = configuration.GetConnectionString("Database");

        // Configure and register Marten
        services.AddMarten(options =>
            {
                options.Connection(connectionString!);
                options.Projections.Add<ShoppingCartSummaryProjection>(ProjectionLifecycle.Inline);
                options.UseSystemTextJsonForSerialization(EnumStorage.AsString, Casing.CamelCase,
                    serializerOptions =>
                    {
                        serializerOptions.Converters.Add(new MoneyConverter());
                    });
            })
            .UseLightweightSessions(); // Use lightweight sessions for better performance

        // Register our repository implementation
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        services.AddScoped<IShoppingCartReadRepository, ShoppingCartReadRepository>();

        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}