using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;

namespace Infrastructure.Repositories;

public class FileRepository(MinioClient client)
{
    public async Task AddFile(IFormFile file, string fileName, string bucketName,
        CancellationToken cancellationToken)
    {
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
    }
}