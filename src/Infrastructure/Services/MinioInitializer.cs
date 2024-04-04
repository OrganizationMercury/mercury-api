using Domain;
using Microsoft.Extensions.Hosting;
using Minio;
using Minio.DataModel.Args;


namespace Infrastructure.Services;

public class MinioInitializer(IMinioClient client) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
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