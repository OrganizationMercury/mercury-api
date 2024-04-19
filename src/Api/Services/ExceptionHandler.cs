using System.Net.Mime;
using System.Text.Json;
using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Api.Services;

public class ExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = GetStatusCode(exception);

        var response = new
        {
            Status = statusCode,
            Detail = exception.Message
        };

        httpContext.Response.ContentType = MediaTypeNames.Application.Json;
        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response), cancellationToken);

        return true;
    }

    private static int GetStatusCode(Exception exception) => exception switch
    {
        ArgumentException => StatusCodes.Status400BadRequest,
        NotFoundException => StatusCodes.Status404NotFound,
        _ => StatusCodes.Status500InternalServerError
    };
}