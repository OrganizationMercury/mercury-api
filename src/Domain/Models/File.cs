

namespace Domain.Models;

public class File
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Bucket { get; set; } = null!;
}