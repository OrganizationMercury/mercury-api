using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Message
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    [MaxLength(40)]
    public string SenderUserName { get; set; } = null!;
    [MaxLength(512)]
    public string Content { get; set; } = null!;
    public DateTime Timestamp { get; set; }
}

