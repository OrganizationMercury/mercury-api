using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public required DbSet<User> Users { get; set; }
}