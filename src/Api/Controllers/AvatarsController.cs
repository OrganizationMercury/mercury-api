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
    public async Task<IActionResult> Get(Guid id, 
        CancellationToken cancellationToken)
    {
        var file = await context.Files
            .Where(file => file.Id == id && file.Bucket == BucketConstants.Avatar) 
            .FirstOrDefaultAsync(cancellationToken);

        if (file is null) return NotFound(Messages.NotFound(nameof(File), id));
        
        var getFileResult = await files.GetFileAsync(file, cancellationToken);
        return getFileResult.Match<IActionResult>(
            stream =>
            {
                var contentType = GetContentType(file.Extension);
                return File(stream, contentType);
            }, error => error switch
            {
                NotFoundError err => NotFound(err.Message),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Not Handled Error")
            });
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