

namespace Domain.Models;

public class File
{
    public string Filename { get; set; } = null!;
    public Guid UserId { get; set; }
    public string Bucket { get; set; } = null!;
}