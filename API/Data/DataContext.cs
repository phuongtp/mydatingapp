using System;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data
{
  public class DataContext: IdentityDbContext<AppUser, AppRole, int, 
          IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, 
          IdentityRoleClaim<int>, IdentityUserToken<int>>
  {
      public DataContext(DbContextOptions<DataContext> options): base(options)
      {            
      }
    // public DbSet<AppUser> Users { get; set; }
    public DbSet<UserLike> Likes {get;set;}
    public DbSet<Message> Messages {get;set;}
    public DbSet<Group> Groups {get;set;}
    public DbSet<Connection> Connections {get;set;}
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      // Identity....
      builder.Entity<AppUser>()
        .HasMany(ur => ur.UserRoles)
        .WithOne(u => u.User)
        .HasForeignKey(ur => ur.UserId)
        .IsRequired();

      builder.Entity<AppRole>()
        .HasMany(ur => ur.UserRoles)
        .WithOne(u => u.Role)
        .HasForeignKey(ur => ur.RoleId)
        .IsRequired();

      // Unique key combination: SourceUserId + LikedUserId
      builder.Entity<UserLike>()
        .HasKey(k => new {k.SourceUserId, k.LikedUserId});

      // One User (SourceUserId) can like many other users (LikedUsers)
      builder.Entity<UserLike>()
        .HasOne(s => s.SourceUser)
        .WithMany(l => l.LikedUsers)
        .HasForeignKey(s => s.SourceUserId)
        .OnDelete(DeleteBehavior.Cascade);

      // One Like User can have many LikedByUsers
      builder.Entity<UserLike>()
        .HasOne(s => s.LikedUser)
        .WithMany(l => l.LikedByUsers)
        .HasForeignKey(s => s.LikedUserId)
        .OnDelete(DeleteBehavior.Restrict); // DeleteBehavior.Cascade generate error!!!

      // Here start Messages
      builder.Entity<Message>()
        .HasOne(u => u.Recipient)
        .WithMany(m => m.MessageReceived)
        .OnDelete(DeleteBehavior.Restrict);

      builder.Entity<Message>()
        .HasOne(u => u.Sender)
        .WithMany(m => m.MessageSent)
        .OnDelete(DeleteBehavior.Restrict);        
    }
  }
}