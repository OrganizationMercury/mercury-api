namespace Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public Guid? AvatarId { get; set; }
    public File? Avatar { get; set; } 
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string? Bio { get; set; }
}