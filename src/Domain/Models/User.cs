using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class User : IdentityUser<Guid>
{
    [MaxLength(40)]
    public string? AvatarFilename { get; set; }
    public File? Avatar { get; set; } 
    
    [MaxLength(40)]
    public string FirstName { get; set; } = null!;
    [MaxLength(40)]
    public string LastName { get; set; } = null!;
    [MaxLength(128)]
    public string? Bio { get; set; }
}