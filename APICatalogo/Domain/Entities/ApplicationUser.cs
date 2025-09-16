using Microsoft.AspNetCore.Identity;

namespace APICatalogo.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}
