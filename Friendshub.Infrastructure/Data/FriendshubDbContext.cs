using Friendshub.Domain.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public virtual DbSet<PostLike> Likes { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }    
        public virtual DbSet<CommentLike> CommentsLikes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.RoleId, ur.UserId });
            modelBuilder.Entity<UserRole>().HasOne(x => x.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRole>().HasOne(x => x.Role).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasData(new Role { Id = 1, Name = "User" });
            modelBuilder.Entity<Role>().HasData(new Role { Id = 2, Name = "Admin" });

            modelBuilder.Entity<PostImage>().HasOne(x => x.Post).WithMany(x => x.PostsImages).HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Follows>().HasKey(follows => new { follows.FollowerId, follows.FolloweeId });

            modelBuilder.Entity<Post>().HasOne(x => x.User).WithMany(u => u.Posts).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Follows>().HasOne(f => f.Follower).WithMany(x => x.Followings).HasForeignKey(f => f.FollowerId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Follows>().HasOne(f => f.Followee).WithMany(u => u.Followers).HasForeignKey(f => f.FolloweeId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostLike>().HasKey(like => new { like.UserId, like.PostId });
            modelBuilder.Entity<PostLike>().HasOne(l => l.Post).WithMany(p => p.Likes).HasForeignKey(l => l.PostId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PostLike>().HasOne(l => l.User).WithMany(x => x.PostLikes).HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>().HasOne(c => c.Post).WithMany(p => p.Comments).HasForeignKey(c => c.PostId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommentLike>().HasKey(cl => new { cl.UserId, cl.CommentId });
            modelBuilder.Entity<CommentLike>().HasOne(x => x.User).WithMany(u => u.CommentLikes).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<CommentLike>().HasOne(x => x.Comment).WithMany(x => x.CommentLikes).HasForeignKey(x => x.CommentId).OnDelete(DeleteBehavior.Cascade);
        }

    }
}
