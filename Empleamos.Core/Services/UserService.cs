using Empleamos.Core.DTOs;
using Empleamos.Core.Entities;
using Empleamos.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empleamos.Core.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserEntity>> GetAllAsync()  
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<bool> UpdateAsync(UserEntity user)  
        {
            return await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> CreateAsync(UserEntity user)  
        {
            return await _userRepository.CreateAsync(user);
        }


        public async Task<UserEntity> GetByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);  
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<UpdateUserResponse> UpdateUserAsync(UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null) return new UpdateUserResponse { Success = false, Message = "User not found." };

            user.FullName = request.FullName;
            user.NIT = request.NIT;
            user.RazonSocial = request.RazonSocial;
            user.Address = request.Address;
            user.City = request.City;
            user.Department = request.Department;
            user.Active = request.Active;
            user.ContactEmail = request.ContactEmail;

            var updateResult = await _userRepository.UpdateAsync(user);
            return updateResult ? new UpdateUserResponse { Success = true, Message = "User updated successfully." } : new UpdateUserResponse { Success = false, Message = "Failed to update user." };
        }


        public Task<List<UserEntity>> GetAll()
        {
            return _userRepository.GetAll();
        }
    }
}
