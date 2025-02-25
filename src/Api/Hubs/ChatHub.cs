using Api.Dto;
using Api.Services;
using Domain.Exceptions;
using Domain.Models;
using Infrastructure;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Api.Hubs;

public class ChatHub(AppDbContext context, ChatService chats) : Hub
{
    public static readonly List<ChatUserDto> Connections = [];
    private static readonly Dictionary<string, string> ConnectionsMap = [];
   
    public async Task SendPrivate(Guid? chatId, Guid interlocutorId, string message)
    {
        if (string.IsNullOrEmpty(message.Trim())) return;
        
        if (chatId is null)
        {
            var sender = await context.Users.FirstOrDefaultAsync(u => u.UserName == IdentityName);
            if (sender is null) throw new NotFoundException(nameof(User), "sender was not found");
            
            var newChatId = await chats.AddPrivateChatAsync(sender.Id, interlocutorId);
            
            chatId = newChatId;
            await Clients.Caller.SendAsync("chatCreated", new { chatId, sender.Id});
        }

        await SendMessage(chatId.Value, message);
    }
    
    public async Task SendGroup(Guid chatId, string message)
    {
        if (string.IsNullOrEmpty(message.Trim())) return;

        await SendMessage(chatId, message);
    }

    public async Task GroupChatCreated(ChatWithAvatarDto chat)
    {
        var realChat = context.Chats.Include(c => c.Users).FirstOrDefault(c => c.Id == chat.Id);
        if (realChat is null) throw new NotFoundException(nameof(Chat), chat.Id);
        
        foreach (var user in realChat.Users)
        {
            if (ConnectionsMap.TryGetValue(user.UserName, out var userId))
            {
                await Clients.Client(userId).SendAsync("chatCreated", chat.Id);
            }
        }
    }
   
    public override Task OnConnectedAsync()
    {
        try
        {
            var user = context.Users.FirstOrDefault(u => u.UserName == IdentityName);
            var userDto = user.Adapt<ChatUserDto>();

            if (Connections.All(u => u.UserName != IdentityName))
            {
                Connections.Add(userDto);
                if (!ConnectionsMap.ContainsKey(IdentityName))
                {
                    ConnectionsMap.Add(IdentityName, Context.ConnectionId);
                }
            }
        }
        catch (Exception ex)
        {
            Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
        }
        return base.OnConnectedAsync();
    } 
    
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var user = Connections.First(u => u.UserName == IdentityName);
            Connections.Remove(user);
            
            ConnectionsMap.Remove(user.UserName);
        }
        catch (Exception ex)
        {
            Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
        }

        return base.OnDisconnectedAsync(exception);
    }
    
    private string IdentityName
    {
        get
        {
            const string type = "name";
            var claim = Context.User!.Claims.FirstOrDefault(c => c.Type == type)?.Value;
            if (claim is not null) return claim;
            throw new ArgumentNullException($"Claim with type: {type} does not exists");
        }
    }
    
    private async Task SendMessage(Guid chatId, string message)
    {
        var chat = await context.Chats
            .Include(chat => chat.Users)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat is null) throw new NotFoundException(nameof(chat), chatId);

        var senderDto = Connections.First(u => u.UserName == IdentityName);
        var dbMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = message.Replace("<", "&lt;").Replace(">", "&gt;"),
            ChatId = chatId,
            SenderUserName = senderDto.UserName,
            Timestamp = DateTime.UtcNow
        };
        
        await context.Messages.AddAsync(dbMessage);
        await context.SaveChangesAsync();

        var messageViewModel = dbMessage.Adapt<MessageDto>();
        
        foreach (var user in chat.Users)
        {
            if (ConnectionsMap.TryGetValue(user.UserName, out var userId))
            {
                await Clients.Client(userId).SendAsync("newMessage", new 
                {
                    message = messageViewModel,
                    receiver = user.UserName 
                });
            }
        }
    }
}