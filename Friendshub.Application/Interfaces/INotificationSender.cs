using Friendshub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Interfaces
{
    public interface INotificationSender
    {
        Task SendAsync(Guid recieverId, Notification notification);
    }
}
