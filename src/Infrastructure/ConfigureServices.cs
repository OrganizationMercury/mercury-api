using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var postgresConnectionString = configuration["POSTGRES_CONNECTION_STRING"] 
                  ?? throw new InvalidOperationException("POSTGRES_CONNECTION_STRING does not exist");
        
        return services
            .AddNeo4J(configuration)
            .AddDbContext<AppDbContext>(options => options.UseNpgsql(postgresConnectionString))
            .AddHostedService<GraphClientInitializer>()
            .AddHostedService<PostgresInitializer>()
            .AddScoped<UserRepository>()
            .AddScoped<InterestRepository>();
    }

    private static IServiceCollection AddNeo4J(this IServiceCollection services, IConfiguration configuration)
    {
        var uri = configuration["NEO4J_URI"] 
                  ?? throw new InvalidOperationException("NEO4J_URI does not exist");
        var username = configuration["NEO4J_USERNAME"]
                       ?? throw new InvalidOperationException("NEO4J_USERNAME does not exist");
        var password = configuration["NEO4J_PASSWORD"] 
                       ?? throw new InvalidOperationException("NEO4J_PASSWORD does not exist");

        var driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password)); 
        return services.AddSingleton(driver);
    }
}