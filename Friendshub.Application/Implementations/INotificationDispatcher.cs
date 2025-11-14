using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Implementations
{
    public interface INotificationDispatcher
    {
        Task DispatchAsync(Guid receiverId, object notification);

    }
}
