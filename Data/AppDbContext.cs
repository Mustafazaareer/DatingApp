using AutoMapper.Execution;
using DatingApp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Member = DatingApp.Entities.Member;

namespace DatingApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
        
    }

    public DbSet<AppUser> Users { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<MemberLike> Likes { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Member)
            .WithOne(p => p.AppUser)
            .HasForeignKey<Member>(p => p.UserId);

        modelBuilder.Entity<MemberLike>()
            .HasKey(m => new { m.SourceMemberId, m.TargetMemberId });
        
        modelBuilder.Entity<MemberLike>()
            .HasOne(u => u.SourceMember)
            .WithMany(p => p.LikedMembers)
            .HasForeignKey(p => p.SourceMemberId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<MemberLike>()
            .HasOne(u => u.TargetMember)
            .WithMany(p => p.LikedByMembers)
            .HasForeignKey(p => p.TargetMemberId)
            .OnDelete(DeleteBehavior.NoAction);
        

        base.OnModelCreating(modelBuilder);

        var dateTimeConverter = new ValueConverter<DateTime,DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v,DateTimeKind.Utc)
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
            }
        }
    }
}