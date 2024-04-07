using Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minio;
using Minio.DataModel.Args;


namespace Infrastructure.Services;

public class MinioInitializer(IServiceScopeFactory scopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IMinioClient>();
        
        var avatarExists = new BucketExistsArgs().WithBucket(BucketConstants.Avatar);
        var exists = await client.BucketExistsAsync(avatarExists, cancellationToken);
        if (!exists)
        {
            var avatarMake = new MakeBucketArgs().WithBucket(BucketConstants.Avatar);
            await client.MakeBucketAsync(avatarMake, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken _) => Task.CompletedTask;
}