using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class User : IdentityUser<Guid>
{
    public override string UserName { get; set; } = null!;
    [MaxLength(40)]
    public string FirstName { get; set; } = null!;
    [MaxLength(40)]
    public string LastName { get; set; } = null!;
    [MaxLength(128)]
    public string? Bio { get; set; }
    public Guid? AvatarId { get; set; }
    public UserAvatar? Avatar { get; set; }
    public List<Chat> Chats { get; set; } = null!;
}