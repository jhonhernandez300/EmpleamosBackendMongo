using Empleamos.Core.DTOs;
using Empleamos.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empleamos.Core.Interfaces
{
    public interface IUserFacadeService
    {
        Task<IEnumerable<UserEntity>> GetAllUsersAsync();
        Task<UserEntity> GetUserByIdAsync(Guid id);
        Task<bool> CreateUserAsync(UserEntity user);
        Task<bool> UpdateUserAsync(Guid id, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(Guid id);
    }
}
