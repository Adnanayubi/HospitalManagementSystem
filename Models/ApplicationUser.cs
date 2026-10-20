using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        public string? RoleDescription { get; set; }

       
    }
}