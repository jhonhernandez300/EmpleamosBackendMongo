using Empleamos.Core.DTOs;
using Empleamos.Core.Entities;
using Empleamos.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empleamos.Core.Facade
{
    public class UserFacadeService : IUserFacadeService
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;

        public UserFacadeService(IUserService userService, IUserRepository userRepository)
        {
            _userService = userService;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserEntity>> GetAllUsersAsync() 
        {
            return await _userService.GetAllAsync();
        }

        public async Task<UserEntity> GetUserByIdAsync(Guid id)
        {
            return await _userService.GetByIdAsync(id);
        }

        public async Task<bool> CreateUserAsync(UserEntity user)
        {
            return await _userService.CreateAsync(user);
        }

        public async Task<bool> UpdateUserAsync(Guid id, UpdateUserRequest request)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return false;

            // Actualizar los campos del usuario desde el request
            user.FullName = request.FullName;
            user.Email = request.ContactEmail;
            user.NIT = request.NIT;
            user.RazonSocial = request.RazonSocial;
            user.Address = request.Address;
            user.City = request.City;
            user.Department = request.Department;
            user.Active = request.Active;

            return await _userService.UpdateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            return await _userService.DeleteAsync(id);
        }
    }


}
