using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empleamos.Core.Entities
{
    [CollectionName("users")]
    public class ApplicationUserEntity : MongoIdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
    }
}
