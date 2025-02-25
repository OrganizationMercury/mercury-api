using Domain.Exceptions;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Infrastructure.Extensions;

public static class MinioClientExtensions
{
    public static async Task<bool> ObjectExistsAsync(this IMinioClient client, ObjectExistsArgs args,
        CancellationToken cancellationToken)
    {
        var statObjectArgs = new StatObjectArgs()
            .WithBucket(args.bucketName)
            .WithObject(args.objectName);

        try
        {
            await client.StatObjectAsync(statObjectArgs, cancellationToken);
        }
        catch (BucketNotFoundException)
        {
            throw new NotFoundException("bucket", args.bucketName);
        }
        catch (ObjectNotFoundException)
        {
            return false;
        }

        return true;
    }
}
public record ObjectExistsArgs(string bucketName, string objectName);
