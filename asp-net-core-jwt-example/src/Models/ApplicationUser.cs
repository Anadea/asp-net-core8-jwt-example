using asp_net_core_jwt_example.Enums;
using Microsoft.AspNetCore.Identity;

namespace asp_net_core_jwt_example.Models;

public class ApplicationUser : IdentityUser
{
    public Role Role { get; set; }
}
