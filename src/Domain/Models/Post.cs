namespace Domain.Models;

public class Post
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ContentId { get; set; }
    public Guid ChatId { get; set; }
    public User User { get; set; }
    public PostContent Content { get; set; }
    public Chat Chat { get; set; }
    public List<Like> Likes { get; set; }
}