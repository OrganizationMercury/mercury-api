namespace Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
}