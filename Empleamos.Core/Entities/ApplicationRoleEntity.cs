﻿using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empleamos.Core.Entities
{
    [CollectionName("roles")]
    public class ApplicationRoleEntity : MongoIdentityRole<Guid>
    {

    }
}
