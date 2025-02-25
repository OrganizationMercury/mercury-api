using Domain;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Models;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class ChatService(AppDbContext context, FileRepository files)
{
    public async Task<Chat> AddChatAsync(List<Guid> userIds, string name, IFormFile? avatar, 
        CancellationToken cancellationToken)
    {
        var users = await context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(cancellationToken: cancellationToken);
        
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            Type = ChatType.Group,
            Users = users,
            Name = name
        };
        
        await context.Chats.AddAsync(chat, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        if (avatar is not null)
        {
            var groupAvatar = await EnsureCreatedGroupAvatarAsync(chat.Id, avatar.FileName, cancellationToken);
            await files
                .PutFileAsync(avatar, groupAvatar, BucketConstants.Avatar, cancellationToken);
            
            chat.Avatar = groupAvatar;
            context.Chats.Update(chat);
            await context.SaveChangesAsync(cancellationToken);
        }

        return chat;
    }
    
    public async Task<Chat> AddCommentsChatAsync(Guid userId, Guid postId, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
        if (user is null) throw new NotFoundException(nameof(User), userId);
        
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            Type = ChatType.Comments,
            Users = [user],
            Avatar = null,
            Name = null
        };
        
        await context.Chats.AddAsync(chat, cancellationToken);

        return chat;
    }
    
    public async Task<Guid> AddPrivateChatAsync(Guid userId, Guid interlocutorId)
    {
        var currentUser = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (currentUser is null) throw new NotFoundException(nameof(User), userId);
        
        var interlocutor = await context.Users.FirstOrDefaultAsync(u => u.Id == interlocutorId);
        if (interlocutor is null) throw new NotFoundException(nameof(User), interlocutorId);
        
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            Type = ChatType.Private,
            Users = [currentUser, interlocutor]
        };

        await context.Chats.AddAsync(chat);
        await context.SaveChangesAsync();

        return chat.Id;
    }

    private async Task<GroupAvatar> EnsureCreatedGroupAvatarAsync(Guid chatId, string filename, CancellationToken cancellationToken)
    {
        var avatar = await context.GroupAvatars.FirstOrDefaultAsync(avatar => avatar.ChatId == chatId, cancellationToken);
        if (avatar is not null) return avatar;
        
        var fileId = Guid.NewGuid();
        var fileExtension = Path.GetExtension(filename);
        avatar = new GroupAvatar
        {
            Filename = $"{fileId}{fileExtension}",
            ChatId = chatId,
            Bucket = BucketConstants.Avatar,
            CreatedAt = DateTime.UtcNow
        };
        await context.GroupAvatars.AddAsync(avatar, cancellationToken);
        
        return avatar;
    }
}