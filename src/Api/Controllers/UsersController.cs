using Api.Dto;
using Api.Services;
using Domain.Abstractions;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Models;
using Infrastructure;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(UserService users, AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var usersList = await users.GetAllAsync(cancellationToken);
        return Ok(usersList);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute]Guid id, 
        CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(id, cancellationToken);
        if (user is null) return NotFound(Messages.NotFound(nameof(user), id));
        
        return Ok(user);
    }
    
    [HttpGet("{username:alpha}")]
    public async Task<IActionResult> GetByUsernameAsync([FromRoute] string username, 
        CancellationToken cancellationToken)
    {
        var user = await users.GetByUsernameAsync(username, cancellationToken);
        if (user is null) return NotFound(Messages.NotFound(nameof(user), username));
        
        return Ok(user);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromForm] UpdateUserDto request,
        CancellationToken cancellationToken)
    {
        await users.UpdateAsync(request, cancellationToken);
        return NoContent();
    }
    
    [HttpGet("{userId:guid}/Chats")]
    public async Task<IActionResult> GetUserChats([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(user => user.Chats)
            .ThenInclude(chat => chat.Users)
            .ThenInclude(user => user.Avatar)
            .Include(user => user.Chats)
            .ThenInclude(chat => chat.Avatar)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        
        if(user is null) return NotFound(Messages.NotFound<User>(userId));
        
        var chats = user.Chats
            .Where(chat => chat.Type != ChatType.Comments)
            .Select(chat => 
            {
                var chatDto = chat.Adapt<ChatWithAvatarDto>();

                if (chatDto.Type is not ChatType.Private)
                {
                    if (chat.Avatar is not null)
                    {
                        chatDto.Avatar = chat.Avatar.Filename;
                    }
                    return chatDto;
                }
                
                var otherUser = chat.Users.FirstOrDefault(u => u.Id != userId);
                if (otherUser is null) throw new NotFoundException(nameof(User), userId);
                    
                chatDto.Name = $"{otherUser.FirstName} {otherUser.LastName}";
                
                if (otherUser.Avatar is not null)
                {
                    chatDto.Avatar = otherUser.Avatar.Filename;
                }
                
                return chatDto;
            }).ToList();
        
        return Ok(chats);
    }
    
    [HttpGet("{userId:guid}/Chats/Private")]
    public async Task<IActionResult> GetUserPrivateChats([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(user => user.Chats)
            .ThenInclude(chat => chat.Users)
            .ThenInclude(user => user.Avatar)
            .Include(user => user.Chats)
            .ThenInclude(chat => chat.Avatar)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        
        if(user is null) return NotFound(Messages.NotFound<User>(userId));
        var privateChats = user.Chats.Where(chat => chat.Type is ChatType.Private);
        
        var chats = privateChats.Select(chat =>
        {
            var chatDto = chat.Adapt<ChatWithAvatarDto>();
            
            var otherUser = chat.Users.FirstOrDefault(u => u.Id != userId);
            if (otherUser is null) throw new NotFoundException(nameof(User), userId);
                
            chatDto.Name = $"{otherUser.FirstName} {otherUser.LastName}";

            if (otherUser.Avatar is not null)
            {
                chatDto.Avatar = otherUser.Avatar.Filename;
            }
            
            return chatDto;
        }).ToList();
        
        return Ok(chats);
    }

    [HttpGet("{userId:guid}/Interlocutors")]
    public async Task<IActionResult> GetUserInterlocutors([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(user => user.Chats)
            .ThenInclude(chat => chat.Users)
            .ThenInclude(user => user.Avatar)
            .Include(user => user.Chats)
            .ThenInclude(chat => chat.Avatar)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        
        if(user is null) return NotFound(Messages.NotFound<User>(userId));
        var privateChats = user.Chats.Where(chat => chat.Type is ChatType.Private);
        
        var userDtos = privateChats.Select(chat =>
        {
            var otherUser = chat.Users.FirstOrDefault(u => u.Id != userId);
            if (otherUser is null) throw new NotFoundException(nameof(User), userId);

            var userDto = new UserWithAvatarDto
            {
                Id = otherUser.Id,
                FullName = $"{otherUser.FirstName} {otherUser.LastName}"
            };

            if (otherUser.Avatar is not null)
            {
                userDto.FileName = otherUser.Avatar.Filename;
            }
            
            return userDto;
        }).ToList();
        
        return Ok(userDtos);
    }
}