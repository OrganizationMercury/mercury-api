using Microsoft.Extensions.Hosting;
using Neo4j.Driver;

namespace Infrastructure.Services;

public class GraphClientInitializer(IDriver driver) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async runner =>
        {
            await runner.RunAsync(
                """
                CREATE CONSTRAINT unique_user_id IF NOT EXISTS FOR (user:User) REQUIRE user.Id IS UNIQUE
                CREATE CONSTRAINT unique_interest_name IF NOT EXISTS FOR (interest:Interest) REQUIRE interest.Name IS UNIQUE
                """);
        });
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        driver.Dispose();
        return Task.CompletedTask;
    }
}