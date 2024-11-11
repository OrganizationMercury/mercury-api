using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public abstract class File
{
    [Key]
    public Guid Id { get; set; } 
    
    [Required]
    public string Filename { get; set; } = null!;
    
    [Required]
    public string Bucket { get; set; } = null!;
    
    [Required]
    public DateTime CreatedAt { get; set; } 
}