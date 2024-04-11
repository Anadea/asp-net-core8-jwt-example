using System.ComponentModel.DataAnnotations;

namespace JwtRoleAuthentication.Models;

public class AuthRequest
{
    [Required] public string? Phone { get; set; }
}