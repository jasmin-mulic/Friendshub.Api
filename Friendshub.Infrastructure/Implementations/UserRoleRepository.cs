using Friendshub.Application.Interfaces;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Infrastructure.Implementations
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly FriendshubDbContext _context;
        public UserRoleRepository(FriendshubDbContext context)
        {
         _context = context;   
        }

        public async Task AddASync(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole);
        }

        public async Task<List<UserRole>> GetRolesByUserId(Guid userId)
        {
            return await _context.UserRoles.AsNoTracking().Include(x => x.User).Where(x => x.UserId == userId).ToListAsync(); 
        }
    }
}
