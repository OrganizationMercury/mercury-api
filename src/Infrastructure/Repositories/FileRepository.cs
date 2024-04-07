using Domain;
using Domain.Abstractions;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;
using OneOf;
using File = Domain.Models.File;

namespace Infrastructure.Repositories;

public class FileRepository(IMinioClient client)
{
    public async Task<OneOf<Ok, Error>> PutFileAsync(IFormFile formFile, File file, string bucketName,
        CancellationToken cancellationToken)
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
        var isBucketExists = await client.BucketExistsAsync(bucketExistsArgs, cancellationToken);
        if (!isBucketExists)
        {
            return Error.NotFound("bucket", bucketName);
        }
        
        await using var stream = new MemoryStream();
        await formFile.CopyToAsync(stream, cancellationToken);
        stream.Position = 0;
        var putObject = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(file.Filename)
            .WithObjectSize(stream.Length)
            .WithStreamData(stream)
            .WithContentType(formFile.ContentType);
        await client.PutObjectAsync(putObject, cancellationToken);

        return new Ok();
    }

    public async Task<OneOf<MemoryStream, Error>> GetFileAsync(File file,
        CancellationToken cancellationToken)
    {
        var args = new ObjectExistsArgs(file.Bucket, "test");
        var result = await client.ObjectAndBucketExistsAsync(args, cancellationToken);
        return await result.Match<Task<OneOf<MemoryStream, Error>>>(
            async _ =>
            {
                var memoryStream = new MemoryStream();

                var objectArgs = new GetObjectArgs()
                    .WithBucket(file.Bucket)
                    .WithObject(file.Filename)
                    .WithCallbackStream(stream => stream.CopyTo(memoryStream));

                await client.GetObjectAsync(objectArgs, cancellationToken);
                return memoryStream;
            }, error => Task.FromResult<OneOf<MemoryStream, Error>>(error)
        );
    }
}