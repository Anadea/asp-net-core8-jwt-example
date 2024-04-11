using System.ComponentModel.DataAnnotations;

namespace asp_net_core_jwt_example.Models;

public class VerifyOtpRequest
{
    [Required] public string? Phone { get; set; }
    [Required] public int? Otp { get; set; }
}
