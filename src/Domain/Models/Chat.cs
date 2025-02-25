using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Models;

public class Chat
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public List<User> Users { get; set; } = null!;
    public ChatType Type { get; set; }
    [MaxLength(40)]
    public string? Name { get; set; }
    public File? Avatar { get; set; }
}