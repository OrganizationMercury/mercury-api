using Api.Dto;
using Domain.Abstractions;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Models;
using Infrastructure;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatsController(AppDbContext context) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetChat([FromQuery] Guid chatId, [FromQuery] Guid currentUserId)
    {
        var chat = await context.Chats
            .Include(chat => chat.Avatar)
            .Include(chat => chat.Users)
            .FirstOrDefaultAsync(u => u.Id == chatId);
        
        if(chat is null) return NotFound(Messages.NotFound<Chat>(chatId));

        var chatDto = chat.Adapt<ChatWithAvatarDto>();
        chatDto.Avatar = chat.Avatar?.Filename;
        
        if (chat.Type is ChatType.Private)
        {
            var otherUser = chat.Users.FirstOrDefault(u => u.Id != currentUserId);
            if (otherUser is null) throw new NotFoundException(nameof(User), currentUserId);
                
            chatDto.Name = $"{otherUser.FirstName} {otherUser.LastName}";
        }

        return Ok(chatDto);
    }
    
    [HttpGet("Private")]
    public async Task<IActionResult> GetPrivateChat([FromQuery] Guid senderId,[FromQuery] Guid receiverId)
    {
        var chat = await context.Chats
            .Include(c => c.Users)
            .Where(c => c.Type == ChatType.Private)
            .FirstOrDefaultAsync(c => c.Users.Any(u => u.Id == senderId) 
                                      && c.Users.Any(u => u.Id == receiverId));
        
        if(chat is null) return NotFound(Messages.NotFound<Chat>($"{senderId} and {receiverId}"));

        var chatDto = chat.Adapt<ChatDto>();
        return Ok(chatDto);
    }
    
    [HttpGet("Private/{chatId:guid}/Users/{senderId:guid}/Interlocutor")]
    public async Task<IActionResult> GetInterlocutor([FromRoute] Guid chatId, [FromRoute] Guid senderId)
    {
        var chat = await context.Chats
            .Include(c => c.Users)
            .FirstOrDefaultAsync(c => c.Type == ChatType.Private && c.Id == chatId);
        
        if(chat is null) return NotFound(Messages.NotFound<Chat>(chatId));

        var user = chat.Users.FirstOrDefault(u => u.Id != senderId);
        var userDto = user.Adapt<ChatUserDto>();
        return Ok(userDto);
    }

    [HttpPost("Private")]
    public async Task<IActionResult> AddPrivateChat(Guid userId, Guid interlocutorId)
    {
        var currentUser = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (currentUser is null) return NotFound(Messages.NotFound<User>(userId));
        
        var interlocutor = await context.Users.FirstOrDefaultAsync(u => u.Id == interlocutorId);
        if (interlocutor is null) return NotFound(Messages.NotFound<User>(interlocutorId));
       
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            Type = ChatType.Private,
            Users = [currentUser, interlocutor]
        };

        await context.Chats.AddAsync(chat);
        await context.SaveChangesAsync();

        return NoContent();
    }
}