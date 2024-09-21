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
        Task<bool> DeleteUserAsync(Guid id);
        Task<UpdateUserResponse> UpdateUserAsync(UpdateUserRequest request);
        Task<List<UserEntity>> GetAll();
    }
}
