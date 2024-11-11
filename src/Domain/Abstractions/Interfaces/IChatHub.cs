using Domain.Models;

namespace Domain.Abstractions.Interfaces;

public interface IChatHub
{
    Task MessageReceivedFromHub(Message message);

    Task NewUserConnected(string message);
}