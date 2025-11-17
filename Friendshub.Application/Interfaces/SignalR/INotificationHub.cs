using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Interfaces.SignalR
{
    public interface INotificationHub
    {
        Task SendNotificationAsync(Guid receiverId, object payload);

    }
}
