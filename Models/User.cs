using Microsoft.AspNetCore.Identity;

namespace FrontToBackMvc.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}
