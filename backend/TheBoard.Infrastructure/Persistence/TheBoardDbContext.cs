using Microsoft.EntityFrameworkCore;
using TheBoard.Core.Models;

namespace TheBoard.Infrastructure.Persistence;

public class TheBoardDbContext(DbContextOptions<TheBoardDbContext> options)
         : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<Ticket> Tickets { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasAlternateKey(u => u.Email);
        modelBuilder.Entity<User>().HasAlternateKey(u => u.Username);
    }
}
