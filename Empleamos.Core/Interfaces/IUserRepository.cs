using Empleamos.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empleamos.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserEntity>> GetAllAsync();
        Task<bool> CreateAsync(UserEntity user);
        Task<bool> DeleteAsync(Guid id);
        Task<UserEntity> GetByIdAsync(Guid id); 
        Task<bool> UpdateAsync(UserEntity user); 
        Task<List<UserEntity>> GetAll();
    }
}
