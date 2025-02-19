using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.UserId).IsRequired();
        builder.Property(p => p.ContentId).IsRequired();
        builder.Property(p => p.ChatId).IsRequired();
        
        builder.HasMany(post => post.Likes)
            .WithOne(like => like.Post)
            .HasForeignKey(like => like.PostId)
            .OnDelete(DeleteBehavior.Cascade);  

        builder.HasOne(post => post.Content)
            .WithOne()
            .HasForeignKey<PostContent>(pc => pc.PostId)
            .OnDelete(DeleteBehavior.Cascade);  

        builder.HasOne(p => p.Chat)
            .WithMany()
            .HasForeignKey(p => p.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(post => post.User)
            .WithMany(user => user.Posts)
            .HasForeignKey(p => p.UserId);
    }
}