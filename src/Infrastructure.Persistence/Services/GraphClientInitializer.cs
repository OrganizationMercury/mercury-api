using Microsoft.Extensions.Hosting;
using Neo4j.Driver;

namespace Infrastructure.Persistence.Services;

public class GraphClientInitializer(IDriver driver) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async runner =>
            await runner.RunAsync("CREATE CONSTRAINT unique_author_id IF NOT EXISTS FOR (user:User) REQUIRE user.Id IS UNIQUE"));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        driver.Dispose();
        return Task.CompletedTask;
    }
}