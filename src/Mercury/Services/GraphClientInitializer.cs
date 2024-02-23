using Neo4j.Driver;

namespace Mercury.Services;

public class GraphClientInitializer : IHostedService
{
    private readonly IDriver _driver;

    public GraphClientInitializer(IDriver driver)
    {
        _driver = driver;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _client.ConnectAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _driver.Dispose();
        return Task.CompletedTask;
    }
}