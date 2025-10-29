using Friendshub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<List<UserRole>> GetRolesByUserId(Guid userId);
        Task AddASync(UserRole userRole);
    }
}
