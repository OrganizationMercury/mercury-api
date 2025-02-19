using Api.Dto;
using Api.Services;
using Domain;
using Domain.Abstractions;
using Domain.Models;
using Infrastructure;
using Infrastructure.Repositories;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController(AppDbContext context, FileRepository files, ChatService chats) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddPost([FromForm] AddPostDto request, CancellationToken cancellationToken)
    {
        var postId = Guid.NewGuid();
        var content = await CreatePostContentAsync(postId, request.File.FileName, cancellationToken);
        var chat = await chats.AddCommentsChatAsync(request.UserId, postId, cancellationToken);

        var post = new Post
        {
            Id = postId,
            UserId = request.UserId,
            Likes = [],
            Content = content,
            ContentId = content.Id,
            Chat = chat,
            ChatId = chat.Id
        };

        await files.PutFileAsync(request.File, content, BucketConstants.Content, cancellationToken);

        await context.Posts.AddAsync(post, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        TypeAdapterConfig<Post, PostDto>.NewConfig()
            .Map(dest => dest.Likes, src => src.Likes.Adapt<List<LikeDto>>())
            .Map(dest => dest.CommentsId, src => src.ChatId);
        var postDto = post.Adapt<PostDto>();
        
        return Ok(postDto);
    }

    [HttpDelete("{postId:guid}")]
    public async Task<IActionResult> DeletePost([FromRoute] Guid postId, CancellationToken cancellationToken)
    {
        var postForDelete = await context.Posts
            .Include(post => post.Likes)
            .Include(post => post.Content)
            .Include(post => post.Chat)
            .FirstOrDefaultAsync(post => post.Id == postId, cancellationToken);
        if (postForDelete is null) return NotFound(Messages.NotFound<Post>(postId));

        await files.DeleteFile(postForDelete.Content, cancellationToken);
        context.Remove(postForDelete);
        await context.SaveChangesAsync(cancellationToken);
        
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPosts([FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        var posts = await context.Posts
            .Include(posts => posts.Likes)
            .Where(post => post.UserId == userId)
            .ToListAsync(cancellationToken);

        foreach (var post in posts)
        {
            Console.WriteLine($"post: {post.Id}");
        }
        TypeAdapterConfig<Post, PostDto>.NewConfig()
            .Map(dest => dest.Likes, src => src.Likes.Adapt<List<LikeDto>>())
            .Map(dest => dest.CommentsId, src => src.ChatId);
        var postDtos = posts.Adapt<List<PostDto>>();
        return Ok(postDtos);
    }

    [HttpPost("{postId:guid}/Like")]
    public async Task<IActionResult> LikePost([FromRoute] Guid postId, [FromBody] Guid userId)
    {
        var post = await context.Posts.FirstOrDefaultAsync(post => post.Id == postId);
        if (post is null) return NotFound(Messages.NotFound<Post>(postId));
        var user = await context.Users.FirstOrDefaultAsync(user => user.Id == userId);
        if (user is null) return NotFound(Messages.NotFound<User>(userId));

        var like = new Like
        {
            Id = Guid.NewGuid(),
            PostId = post.Id,
            UserId = user.Id,
            Post = post,
            User = user
        };

        await context.AddAsync(like);
        await context.SaveChangesAsync();
        return Ok(like.Id);
    }

    [HttpDelete("{postId:guid}/Like")]
    public async Task<IActionResult> UnlikePost([FromRoute] Guid postId, [FromQuery] Guid userId)
    {
        var like = await context.Likes
            .FirstOrDefaultAsync(like => 
                like.PostId == postId &&
                like.UserId == userId);
        if (like is null) return NotFound(Messages.NotFound<Like>($"post and user ids: {postId}, {userId}"));
        
        context.Remove(like);
        await context.SaveChangesAsync();
        return Ok(like.Id);
    }

    [HttpGet("{postId:guid}/Like")]
    public async Task<IActionResult> GetLike([FromRoute] Guid postId, [FromQuery] Guid userId)
    {
        var like = await context.Likes
            .FirstOrDefaultAsync(like => 
                like.PostId == postId &&
                like.UserId == userId);
        if (like is null) return NotFound(Messages.NotFound<Like>($"post and user ids: {postId}, {userId}"));

        return Ok(like);
    }
    
    private async Task<PostContent> CreatePostContentAsync(Guid postId, string filename, CancellationToken cancellationToken)
    {
        var fileId = Guid.NewGuid();
        var fileExtension = Path.GetExtension(filename);
        var postContent = new PostContent
        {
            Id = fileId,
            Filename = $"{fileId}{fileExtension}",
            PostId = postId,
            Bucket = BucketConstants.Content,
            CreatedAt = DateTime.UtcNow
        };
        await context.PostsContent.AddAsync(postContent, cancellationToken);
        
        return postContent;
    }
}