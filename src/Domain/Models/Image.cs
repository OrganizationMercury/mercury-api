namespace Domain.Models;

public class Image
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ImageBytes { get; set; } = null!;
}