using Api.Dto;
using Api.Services;
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
public class ChatsController(AppDbContext context, ChatService chats) : ControllerBase
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
            .ThenInclude(user => user.Avatar)
            .FirstOrDefaultAsync(c => c.Type == ChatType.Private && c.Id == chatId);
        
        if(chat is null) return NotFound(Messages.NotFound<Chat>(chatId));

        var user = chat.Users.FirstOrDefault(u => u.Id != senderId);
        var userDto = user.Adapt<ChatUserDto>();
        userDto.FileName = user?.Avatar?.Filename;
        return Ok(userDto);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddChat([FromForm] AddChatDto dto, 
        CancellationToken cancellationToken)
    {
        var chat = await chats.AddChatAsync(dto.UserIds, dto.Name, dto.Avatar, cancellationToken);
        var chatDto = chat.Adapt<ChatWithAvatarDto>();
        chatDto.Avatar = chat.Avatar?.Filename;
        
        return Ok(chatDto);
    }
}