using System.ComponentModel.DataAnnotations;

namespace asp_net_core_jwt_example.Models;

public class AuthRequest
{
    [Required] public string? Phone { get; set; }
}
