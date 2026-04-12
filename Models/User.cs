using Microsoft.AspNetCore.Identity;

namespace Marketeer.Models;

public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Role { get; set; } = "Customer"; 
    public string Status { get; set; } = "Active";
}
