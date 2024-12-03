using Microsoft.CodeAnalysis.CSharp.Syntax;
using FrontToBackMvc.Models;
using Microsoft.AspNetCore.Identity;
using FrontToBackMvc.DataAccess;
using FrontToBackMvc.Enums;

namespace FrontToBackMvc.Extentions
{
    public static class SeedExtention
    {
        public static void UseUserSeed(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var userMeneger = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleMeneger = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if(!roleMeneger.Roles.Any())
                {
                    foreach (Roles item in Enum.GetValues(typeof(Roles)))
                    {
                        roleMeneger.CreateAsync(new IdentityRole(item.ToString())).Wait();
                    }
                }

                if (!userMeneger.Users.Any(x => x.NormalizedUserName == "ADMIN"))
                {

                    User u = new User
                    {
                        FullName = "admin",
                        UserName = "admin",
                        Email = "admin@gmail.com",
                        ProfileImageUrl = "ProfilePhoto.img",
                    };
                    userMeneger.CreateAsync(u,"123").Wait();
                    userMeneger.AddToRoleAsync(u, nameof(Roles.Admin)).Wait();
                }


            }
        }
    }
}
