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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        builder.Entity<User>()
            .HasOne<File>(user => user.Avatar)
            .WithOne()
            .HasForeignKey<User>(user => user.AvatarFilename);
        
        builder.Entity<File>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<File>(i => i.UserId);
    }
}