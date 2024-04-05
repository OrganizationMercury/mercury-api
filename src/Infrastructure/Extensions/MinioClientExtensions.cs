using Domain.Abstractions;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using OneOf;

namespace Infrastructure.Extensions;

public static class MinioClientExtensions
{
    public static async Task<OneOf<Ok, Error>> ObjectAndBucketExistsAsync(this IMinioClient client, ObjectExistsArgs args,
        CancellationToken cancellationToken)
    {
        var statObjectArgs = new StatObjectArgs()
            .WithBucket(args.bucketName)
            .WithObject(args.objectName);

        try
        {
            await client.StatObjectAsync(statObjectArgs, cancellationToken);
        }
        catch (ObjectNotFoundException)
        {
            return Error.NotFound("file", args.objectName);
        }
        catch (BucketNotFoundException)
        {
            return Error.NotFound("bucket", args.bucketName);
        }

        return new Ok();
    }
}
public record ObjectExistsArgs(string bucketName, string objectName);
