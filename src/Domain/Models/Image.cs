

namespace Domain.Models;

public class Image
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Path { get; set; } = null!;
}