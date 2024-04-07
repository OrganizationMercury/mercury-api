using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
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
            .AddMinio(configuration)
            .AddDbContext<AppDbContext>(options => options.UseNpgsql(postgresConnectionString))
            .AddHostedService<MinioInitializer>()
            .AddHostedService<GraphClientInitializer>()
            .AddHostedService<PostgresInitializer>()
            .AddScoped<UserRepository>()
            .AddScoped<InterestRepository>()
            .AddScoped<FileRepository>();
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

    private static IServiceCollection AddMinio(this IServiceCollection services, IConfiguration configuration)
    {
        var user = configuration["MINIO_ROOT_USER"]
                   ?? throw new InvalidOperationException("MINIO_ROOT_USER does not exist");
        var password = configuration["MINIO_ROOT_PASSWORD"]
                       ?? throw new InvalidOperationException("MINIO_ROOT_PASSWORD does not exist");

        return services.AddScoped<IMinioClient>(_ => new MinioClient()
            .WithEndpoint("minio:9000")
            .WithCredentials(user, password)
            .WithSSL(false)
            .Build()
        );
    }
}