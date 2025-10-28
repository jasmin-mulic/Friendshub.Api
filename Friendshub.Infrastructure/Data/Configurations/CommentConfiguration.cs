using Friendshub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Friendshub.Infrastructure.Data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasOne(c => c.Post).WithMany(p => p.Comments).HasForeignKey(c => c.PostId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(c => c.User).WithMany(u => u.Comments).HasForeignKey((x => x.UserId)).OnDelete(DeleteBehavior.NoAction);
            builder.HasQueryFilter(x => x.IsActive && !x.IsDeleted);

        }
    }
}
