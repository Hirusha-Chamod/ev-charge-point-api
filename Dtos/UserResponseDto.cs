using ev_charge_point_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ev_charge_point_api.Dtos
{
    public class UserResponseDto
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public RoleType Role { get; set; }
    }
}
