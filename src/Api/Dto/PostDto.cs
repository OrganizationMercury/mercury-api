using Domain.Models;

namespace Api.Dto;

public class AddPostDto
{
    public Guid UserId { get; set; }
    public IFormFile File { get; set; }
}

public class PostDto
{
    public Guid Id { get; set; }
    public Guid ContentId { get; set; }
    public List<LikeDto> Likes { get; set; }
    public Guid CommentsId { get; set; }
}

public class LikeDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
}