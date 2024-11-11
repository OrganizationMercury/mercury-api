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
    public async Task<List<MessageDto>> GetMessages([FromQuery] Guid chatId) =>
        await context.Messages
            .Where(m => m.ChatId == chatId)
            .ProjectToType<MessageDto>()
            .ToListAsync();
}