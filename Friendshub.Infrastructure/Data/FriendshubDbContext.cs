using Friendshub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Infrastructure.Data
{
    public class FriendshubDbContext : DbContext
    {
        public FriendshubDbContext(DbContextOptions<FriendshubDbContext> options) : base(options)
        {

        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<PostImage> PostImages { get; set; }
        public virtual DbSet<Follows> Follows { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.RoleId, ur.UserId });
            modelBuilder.Entity<UserRole>().HasOne(x => x.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRole>().HasOne(x => x.Role).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Role>().HasData(new Role { Id = 1, Name = "User" });
            modelBuilder.Entity<Role>().HasData(new Role { Id = 2, Name = "Admin" });
            modelBuilder.Entity<PostImage>().HasOne(x => x.Post).WithMany(x => x.PostsImages).HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.Cascade);


                modelBuilder.Entity<Follows>().HasKey(follows => new { follows.FollowerId, follows.FolloweeId });

                modelBuilder.Entity<Follows>().HasOne(f => f.Follower).WithMany(x => x.Followings).HasForeignKey(f => f.FollowerId).OnDelete(DeleteBehavior.Restrict);
                modelBuilder.Entity<Follows>().HasOne(f => f.Followee).WithMany(u => u.Followers).HasForeignKey(f => f.FolloweeId).OnDelete(DeleteBehavior.Cascade);

        }

    }
}
