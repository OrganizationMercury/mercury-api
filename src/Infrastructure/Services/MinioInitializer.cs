using Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel.Args;


namespace Infrastructure.Services;

public class MinioInitializer(IServiceScopeFactory scopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IMinioClient>();
        
        await EnsureBucketExistsAsync(client, BucketConstants.Avatar, cancellationToken);
        await EnsureBucketExistsAsync(client, BucketConstants.Content, cancellationToken);
    }

    public Task StopAsync(CancellationToken _) => Task.CompletedTask;
    
    private static async Task EnsureBucketExistsAsync(IBucketOperations client, string bucketName, CancellationToken cancellationToken)
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
        var isBucketExists = await client.BucketExistsAsync(bucketExistsArgs, cancellationToken);
        if (!isBucketExists)
        {
            var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
            await client.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }
    }
}