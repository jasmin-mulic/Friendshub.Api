using Friendshub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Friendshub.Infrastructure.Data.Configurations
{
    public class PostImageConfiguration : IEntityTypeConfiguration<PostImage>
    {
        public void Configure(EntityTypeBuilder<PostImage> builder)
        {
            builder.HasOne(x => x.Post).WithMany(p => p.PostsImages).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
