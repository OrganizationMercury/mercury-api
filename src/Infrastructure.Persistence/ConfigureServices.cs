using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Services;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;

namespace Infrastructure.Persistence;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services
            .AddNeo4J()
            .AddHostedService<GraphClientInitializer>()
            .AddScoped<UserRepository>();
    }

    private static IServiceCollection AddNeo4J(this IServiceCollection services)
    {
        var uri = Environment.GetEnvironmentVariable("NEO4J_URI") 
                  ?? throw new InvalidOperationException("NEO4J_URI does not exist");
        var username = Environment.GetEnvironmentVariable("NEO4J_USERNAME")
                       ?? throw new InvalidOperationException("NEO4J_USERNAME does not exist");
        var password = Environment.GetEnvironmentVariable("NEO4J_PASSWORD") 
                       ?? throw new InvalidOperationException("NEO4J_PASSWORD does not exist");

        var driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password)); 
        return services.AddSingleton(driver);
    }
}