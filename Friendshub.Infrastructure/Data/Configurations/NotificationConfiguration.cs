using Friendshub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Friendshub.Infrastructure.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {

            builder.HasOne(x => x.Sender)
                .WithMany(s => s.SentNotifications)
                .HasForeignKey(s => s.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Receiver)
                .WithMany(s => s.Notifications)
                .HasForeignKey(s => s.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
