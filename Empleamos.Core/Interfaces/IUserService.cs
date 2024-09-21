using Empleamos.Core.DTOs;
using Empleamos.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empleamos.Core.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserEntity>> GetAllAsync();
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdateAsync(UserEntity user);
        Task<bool> CreateAsync(UserEntity user);
        Task<UserEntity> GetByIdAsync(Guid id);
        Task<bool> DeleteUserAsync(Guid id);
        Task<UpdateUserResponse> UpdateUserAsync(UpdateUserRequest request);
        Task<List<UserEntity>> GetAll();
    }
}
