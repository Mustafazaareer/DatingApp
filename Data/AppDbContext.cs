using Microsoft.EntityFrameworkCore;
using Task.Entities;

namespace Task.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
        
    }

    public DbSet<AppUser> Users { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Photo> Photos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Member)
            .WithOne(p => p.AppUser)
            .HasForeignKey<Member>(p => p.UserId);

        base.OnModelCreating(modelBuilder);
    }
}