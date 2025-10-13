using Friendshub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Friendshub.Infrastructure.Data.Configurations
{
    public class FollowRequestConfiguration : IEntityTypeConfiguration<FollowRequest>
    {
        public void Configure(EntityTypeBuilder<FollowRequest> builder)
        {
            builder.HasKey(f => new { f.SenderId, f.RecieverId });

             builder.HasOne(f => f.Sender)
                    .WithMany(x => x.SentFollowRequests)
                    .HasForeignKey(f => f.SenderId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Reciever)
                .WithMany(x => x.RecievedFollowRequest)
                .HasForeignKey(x => x.RecieverId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
