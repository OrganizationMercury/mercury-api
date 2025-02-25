using Api.Dto;
using Infrastructure;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MessagesController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMessages(
        [FromQuery] Guid chatId,
        [FromQuery] bool latest = false) 
    {
        if (latest)
        {
            var lastMessage = await context.Messages
                .Where(message => message.ChatId == chatId)
                .OrderByDescending(message => message.Timestamp)
                .AsNoTracking()
                .ProjectToType<MessageDto>()
                .FirstOrDefaultAsync();

            return Ok(lastMessage); 
        }
        
        var messages = await context.Messages
            .Where(message => message.ChatId == chatId)
            .AsNoTracking()
            .ProjectToType<MessageDto>()
            .ToListAsync();

        return Ok(messages);
    }
}