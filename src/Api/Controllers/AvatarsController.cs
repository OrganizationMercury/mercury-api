using Domain;
using Domain.Abstractions;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AvatarsController(FileRepository files, AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(string filename, 
        CancellationToken cancellationToken)
    {
        var file = await context.Files
            .Where(file => file.Filename == filename && file.Bucket == BucketConstants.Avatar) 
            .FirstOrDefaultAsync(cancellationToken);

        if (file is null) return NotFound(Messages.NotFound(nameof(File), filename));
        
        var memoryStream = await files.GetFileAsync(file, cancellationToken);
        var contentType = GetContentType(file.Filename);
        return File(memoryStream, contentType);
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> GetByUserId(Guid userId, 
        CancellationToken cancellationToken)
    {
        var file = await context.Files
            .Where(file => file.UserId == userId && file.Bucket == BucketConstants.Avatar) 
            .FirstOrDefaultAsync(cancellationToken);

        if (file is null) return NotFound(Messages.NotFound(nameof(File), userId));
        
        var memoryStream = await files.GetFileAsync(file, cancellationToken);
        memoryStream.Seek(0, SeekOrigin.Begin);
        var contentType = GetContentType(file.Filename);
        return File(memoryStream, contentType);
    }

    private static string GetContentType(string source)
    {
        var provider = new FileExtensionContentTypeProvider();
        if(!provider.TryGetContentType(source, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return contentType;
    }
}