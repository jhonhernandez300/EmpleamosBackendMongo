using Empleamos.Core.Entities;
using Empleamos.Core.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empleamos.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> _users;

        public UserRepository(IOptions<MongoDbSettingsEntity> options)
        {
            var mongoClient = new MongoClient(options.Value.ConnectionString);
            _users = mongoClient.GetDatabase(options.Value.DatabaseName).GetCollection<UserEntity>("users");
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var result = await _users.DeleteOneAsync(user => user.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<UserEntity> GetByIdAsync(Guid id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(UserEntity user)
        {
            var result = await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<List<UserEntity>> GetAll() => await _users.Find(_ => true).ToListAsync();
    }
}
