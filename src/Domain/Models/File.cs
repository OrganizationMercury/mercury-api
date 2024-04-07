using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class File
{
    [Key]
    public string Filename { get; set; } = null!;
    public Guid UserId { get; set; }
    public string Bucket { get; set; } = null!;
}