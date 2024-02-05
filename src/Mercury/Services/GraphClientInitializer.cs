using Neo4jClient;

namespace Mercury.Services;

public class GraphClientInitializer : IHostedService
{
    private readonly IGraphClient _client;

    public GraphClientInitializer(IGraphClient client)
    {
        _client = client;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _client.ConnectAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _client.Dispose();
        return Task.CompletedTask;
    }
}