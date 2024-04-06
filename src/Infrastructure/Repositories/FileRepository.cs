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
    public async Task<OneOf<File, Error>> AddFileAsync(IFormFile file, Guid userId, string bucketName,
        CancellationToken cancellationToken)
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
        var isBucketExists = await client.BucketExistsAsync(bucketExistsArgs, cancellationToken);
        if (!isBucketExists)
        {
            return Error.NotFound("bucket", bucketName);
        }
        
        var fileId = Guid.NewGuid();
        var fileExtension = Path.GetExtension(file.FileName);
        var fileName = $"{fileId}{fileExtension}";
        
        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);
        stream.Position = 0;
        var putObject = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithFileName(fileName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(file.ContentType);
        await client.PutObjectAsync(putObject, cancellationToken);
        
        return new File
        {
            Id = fileId,
            UserId = userId,
            Bucket = BucketConstants.Avatar,
            Extension = fileExtension
        };
    }

    public async Task<OneOf<MemoryStream, Error>> GetFileAsync(File file,
        CancellationToken cancellationToken)
    {
        var fileName = $"{file.Id}{file.Extension}";
        var args = new ObjectExistsArgs(file.Bucket, "test");
        var result = await client.ObjectAndBucketExistsAsync(args, cancellationToken);
        return await result.Match<Task<OneOf<MemoryStream, Error>>>(
            async _ =>
            {
                var memoryStream = new MemoryStream();

                var objectArgs = new GetObjectArgs()
                    .WithBucket(file.Bucket)
                    .WithObject(fileName)
                    .WithCallbackStream(stream => stream.CopyTo(memoryStream));

                await client.GetObjectAsync(objectArgs, cancellationToken);
                return memoryStream;
            }, error => Task.FromResult<OneOf<MemoryStream, Error>>(error)
        );
    }
}