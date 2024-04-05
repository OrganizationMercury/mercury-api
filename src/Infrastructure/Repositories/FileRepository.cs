using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;

namespace Infrastructure.Repositories;

public class FileRepository(IMinioClient client)
{
    public async Task<OneOf<Guid, Error>> AddFile(IFormFile file, string bucketName,
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
        return fileId;
    }
}