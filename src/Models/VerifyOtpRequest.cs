using System.ComponentModel.DataAnnotations;

namespace JwtRoleAuthentication.Models;

public class VerifyOtpRequest
{
    [Required] public string? Phone { get; set; }
    [Required] public int? Otp { get; set; }
}