using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empleamos.Core.DTOs
{
    public class UpdateUserRequest
    {
        public Guid Id { get; set; } 
        public string FullName { get; set; } = string.Empty;
        public string NIT { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public bool Active { get; set; } 
        public string ContactEmail { get; set; } = string.Empty; 
    }
}
