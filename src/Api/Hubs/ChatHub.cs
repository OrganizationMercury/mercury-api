using Api.Dto;
using Domain.Enums;
using Domain.Models;
using Infrastructure;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Api.Hubs;

public class ChatHub(AppDbContext context) : Hub
{
    public static readonly List<ChatUserDto> Connections = [];
    private static readonly Dictionary<string, string> ConnectionsMap = [];
   
    public async Task SendPrivate(Guid? chatId, Guid interlocutorId, string message)
    {
        if (chatId is null)
        {
            var sender = await context.Users.FirstOrDefaultAsync(u => u.UserName == IdentityName);
            if (sender is null) throw new NotImplementedException("sender was not found");
            
            var interlocutor = await context.Users.FirstOrDefaultAsync(u => u.Id == interlocutorId);
            if (interlocutor is null) throw new NotImplementedException("sender was not found");

            var newChat = new Chat
            {
                Id = Guid.NewGuid(),
                Type = ChatType.Private,
                Users = [sender, interlocutor]
            };

            await context.Chats.AddAsync(newChat);
            await context.SaveChangesAsync();
            
            chatId = newChat.Id;
            await Clients.Caller.SendAsync("chatCreated", chatId);
        }
        
        var chat = await context.Chats
            .Include(chat => chat.Users)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat is null) throw new NotImplementedException("chat was not found");
        if (string.IsNullOrEmpty(message.Trim())) return;

        var senderDto = Connections.First(u => u.UserName == IdentityName);
        var dbMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = message.Replace("<", "&lt;").Replace(">", "&gt;"),
            ChatId = chatId.Value,
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
                await Clients.Client(userId).SendAsync("newMessage", messageViewModel);
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
}