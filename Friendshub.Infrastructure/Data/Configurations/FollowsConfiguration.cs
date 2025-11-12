using Friendshub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Friendshub.Infrastructure.Data.Configurations
{
    public class FollowsConfiguration : IEntityTypeConfiguration<Follow>
    {
        public void Configure(EntityTypeBuilder<Follow> builder)
        {
            builder.HasKey(f => new { f.FollowerId, f.FolloweeId });

            builder.HasOne(f => f.Follower)
                .WithMany(x => x.Followings)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(f => f.Followee)
                .WithMany(u => u.Followers).
                HasForeignKey(f => f.FolloweeId).
                OnDelete(DeleteBehavior.Cascade);
        }
    }
}
