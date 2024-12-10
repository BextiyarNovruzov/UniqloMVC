using FrontToBackMvc.DataAccess;
using FrontToBackMvc.Extentions;
using FrontToBackMvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FrontToBackMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<UniqloDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("MySql")));


            builder.Services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";
                opt.Password.RequiredLength = 3;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Lockout.MaxFailedAccessAttempts = 3;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

            }).AddDefaultTokenProviders().AddEntityFrameworkStores<UniqloDbContext>();
            //builder.Services.ConfigureApplicationCookie(x =>
            //{
            //    x.AccessDeniedPath = "/Home/AccessDenied";
            //});

            var app = builder.Build();

          
            app.UseUserSeed();

            app.MapControllerRoute(
               name: "Register",
                              pattern: "Register",

                defaults:new  {controller= "Account" ,action= "Register"});

            app.MapControllerRoute(
               name: "Login",
                              pattern: "Login",

                defaults: new { controller = "Account", action = "Login" });


            app.MapControllerRoute(
                name: "area",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");


            app.MapControllerRoute(name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.UseStaticFiles();

            app.Run();
        }
    }
}
