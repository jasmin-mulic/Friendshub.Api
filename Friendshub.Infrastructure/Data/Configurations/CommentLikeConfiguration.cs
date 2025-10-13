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
    public class CommentLikeConfiguration : IEntityTypeConfiguration<CommentLike>
    {
        public void Configure(EntityTypeBuilder<CommentLike> builder)
        {

            builder.HasKey(cl => new { cl.UserId, cl.CommentId });
            builder.HasOne(x => x.User).WithMany(u => u.CommentLikes).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Comment).WithMany(x => x.CommentLikes).HasForeignKey(x => x.CommentId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
