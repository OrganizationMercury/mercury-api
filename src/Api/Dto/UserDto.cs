namespace Api.Dto;

public record UpdateUserDto(
    Guid Id,
    string Firstname,
    string Lastname,
    string Username,
    string? Bio,
    IFormFile? File);

public class ChatUserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } 
    public string FirstName { get; set; } 
    public string LastName { get; set; } 
    public string? Bio { get; set; }
    public string? FileName { get; set; } 
}
    
public class UserWithAvatarDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string? FileName { get; set; }
}