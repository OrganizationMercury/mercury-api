using Domain.Exceptions;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;
using File = Domain.Models.File;

namespace Infrastructure.Repositories;

public class FileRepository(IMinioClient client)
{
    public async Task PutFileAsync(IFormFile formFile, File file, string bucketName,
        CancellationToken cancellationToken)
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
        var isBucketExists = await client.BucketExistsAsync(bucketExistsArgs, cancellationToken);
        if (!isBucketExists)
        {
            throw new NotFoundException("bucket", bucketName);
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
    }

    public async Task<MemoryStream> GetFileAsync(File file,
        CancellationToken cancellationToken)
    {
        var objectExistsArgs = new ObjectExistsArgs(file.Bucket, file.Filename);
        var objectExists = await client.ObjectExistsAsync(objectExistsArgs, cancellationToken);
        if(!objectExists) throw new NotFoundException(nameof(File), file.Filename);
        
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(file.Bucket);
        var bucketExists = await client.BucketExistsAsync(bucketExistsArgs, cancellationToken);
        if(!bucketExists) throw new NotFoundException("bucket", file.Bucket);
        
        var memoryStream = new MemoryStream();

        var objectArgs = new GetObjectArgs()
            .WithBucket(file.Bucket)
            .WithObject(file.Filename)
            .WithCallbackStream(stream => stream.CopyTo(memoryStream));

        await client.GetObjectAsync(objectArgs, cancellationToken);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }
}