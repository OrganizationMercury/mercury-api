using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public required DbSet<User> Users { get; set; }
    public required DbSet<Image> Images { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        builder.Entity<User>()
            .HasOne<Image>(user => user.Avatar)
            .WithOne()
            .HasForeignKey<User>(u => u.AvatarId);
        
        builder.Entity<Image>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Image>(i => i.UserId);
    }
}