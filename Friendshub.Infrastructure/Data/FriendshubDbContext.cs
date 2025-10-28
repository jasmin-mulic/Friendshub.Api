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
        public virtual DbSet<Follow> Follows { get; set; }
        public virtual DbSet<PostLike> Likes { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }    
        public virtual DbSet<CommentLike> CommentsLikes { get; set; }
        public virtual DbSet<FollowRequest> FollowRequests { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FriendshubDbContext).Assembly);
        }
    }
}
