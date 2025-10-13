using Friendshub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Infrastructure.Data.Configurations
{
    public class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
    {
        public void Configure(EntityTypeBuilder<PostLike> builder)
        {

            builder.HasKey(like => new { like.UserId, like.PostId });
            builder.HasOne(l => l.Post).WithMany(p => p.Likes).HasForeignKey(l => l.PostId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(l => l.User).WithMany(x => x.PostLikes).HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
