using Domain;
using Domain.Abstractions;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
public class AvatarsController(FileRepository files, AppDbContext context) : ControllerBase
{
    [HttpGet("Avatars/{filename}")]
    public async Task<IActionResult> Get([FromRoute] string filename, 
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
    
    [HttpGet("Files/{fileId:guid}")]
    public async Task<IActionResult> GetByFileId([FromRoute] Guid fileId, 
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"FILEID: {fileId}");
        var file = await context.Files
            .Where(file => file.Id == fileId) 
            .FirstOrDefaultAsync(cancellationToken);
        
        if (file is null) return NotFound(Messages.NotFound(nameof(File), fileId));
        Console.WriteLine($"FILE: {file.Filename}");
        Console.WriteLine($" {file.Bucket}");
        var memoryStream = await files.GetFileAsync(file, cancellationToken);
        var contentType = GetContentType(file.Filename);
        return File(memoryStream, contentType);
    }
    
    [HttpGet("Users/{userId:guid}/Avatar")]
    public async Task<IActionResult> GetByUserId([FromRoute] Guid userId, 
        CancellationToken cancellationToken)
    {
        var file = await context.UserAvatars
            .Where(file => file.UserId == userId && file.Bucket == BucketConstants.Avatar) 
            .FirstOrDefaultAsync(cancellationToken);

        if (file is null) return NotFound(Messages.NotFound(nameof(File), userId));
        
        var memoryStream = await files.GetFileAsync(file, cancellationToken);
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