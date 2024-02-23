using Neo4j.Driver;

namespace Mercury.Services;

public class GraphClientInitializer : IHostedService
{
    private readonly IDriver _driver;

    public GraphClientInitializer(IDriver driver) => _driver = driver;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(async runner =>
            await runner.RunAsync("CREATE CONSTRAINT unique_author_id IF NOT EXISTS FOR (user:User) REQUIRE user.Id IS UNIQUE"));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _driver.Dispose();
        return Task.CompletedTask;
    }
}