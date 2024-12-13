using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using File = Domain.Models.File;

namespace Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) 
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public required DbSet<File> Files { get; set; }
    public DbSet<UserAvatar> UserAvatars { get; set; }
    public DbSet<GroupAvatar> GroupAvatars { get; set; }
    public required DbSet<Message> Messages { get; set; }
    public required DbSet<Chat> Chats { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        builder.Entity<User>()
            .HasOne<UserAvatar>(user => user.Avatar)
            .WithOne()
            .HasForeignKey<User>(user => user.AvatarId);

        builder.Entity<Chat>()
            .HasMany(c => c.Users)
            .WithMany(u => u.Chats);
        
        builder.Entity<File>()
            .HasDiscriminator<string>("FileType")
            .HasValue<UserAvatar>("UserAvatar")
            .HasValue<GroupAvatar>("GroupAvatar");
        
        builder.Entity<UserAvatar>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<UserAvatar>(avatar => avatar.UserId);
        
        builder.Entity<GroupAvatar>()
            .HasOne<Chat>()
            .WithOne()
            .HasForeignKey<GroupAvatar>(avatar => avatar.ChatId);
    }
}