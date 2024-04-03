using Domain.Models;
using Microsoft.EntityFrameworkCore;
using File = Domain.Models.File;

namespace Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public required DbSet<User> Users { get; set; }
    public required DbSet<File> Images { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        builder.Entity<User>()
            .HasOne<File>(user => user.Avatar)
            .WithOne()
            .HasForeignKey<User>(u => u.AvatarId);
        
        builder.Entity<File>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<File>(i => i.UserId);
    }
}