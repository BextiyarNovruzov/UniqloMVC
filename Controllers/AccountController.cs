using FrontToBackMvc.Enums;
using FrontToBackMvc.Helpers;
using FrontToBackMvc.Models;
using FrontToBackMvc.ViewModels.Auths;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Mail;

namespace FrontToBackMvc.Controllers
{
    public class AccountController(UserManager<User> _userManager, SignInManager<User> _signinManager) : Controller
    {
        //readonly SmtpOptions smtpoptions = options.Value;
        bool isAuthenticated => User.Identity?.IsAuthenticated ?? false;

        public IActionResult Register()
        {
            if (isAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserVM vm)
        {
            if (isAuthenticated) return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid) return View();
            User user = new User
            {
                UserName = vm.UserName,
                Email = vm.Email,
                FullName = vm.FullName,
                ProfileImageUrl = "ProfilePhoto.img"
            };
            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            var RoleResult = await _userManager.AddToRoleAsync(user, nameof(Roles.User));
            if (!RoleResult.Succeeded)
            {
                foreach (var error in RoleResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            return RedirectToAction("Login", "Account");


        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm, string? returnUrl = null)
        {
            if (isAuthenticated) return RedirectToAction("Index", "Home");
            if (!ModelState.IsValid) return View();
            User? user = null;
            if (vm.UserNameOrEmail.Contains('@'))
                user = await _userManager.FindByEmailAsync(vm.UserNameOrEmail);
            else
                user = await _userManager.FindByNameAsync(vm.UserNameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError("", "UserName or Password is wrong.");
                return View();
            }

            var result = await _signinManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, true);

            if (!result.Succeeded)
            {
                if (!result.IsLockedOut && !result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "UserName or Password is wrong.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "UserName or Password is wrong.");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Wait Until " + user.LockoutEnd!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                return View();
            }

            if (string.IsNullOrEmpty(returnUrl))
            {
                if (await _userManager.IsInRoleAsync(user, "admin")) 
                {
                    return RedirectToAction("Index", "Dashboard", new { Area = "Admin" });
                }
                return RedirectToAction("Index", "Home");
            }

            return LocalRedirect(returnUrl);

        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return View(nameof(Login));
        }

        //public async Task<IActionResult> Send()
        //{
        //    //SmtpClient smtp = new SmtpClient();
        //    //smtp.Host = smtpoptions.Host;
        //    //smtp.Port = smtpoptions.Port;
        //    //smtp.EnableSsl = true;  
        //    //smtp.Credentials = new NetworkCredential(smtpoptions.UserName, smtpoptions.Password);
        //    //MailAddress from = new MailAddress(smtpoptions.UserName, "Elvet Steakhouse");
        //    //MailAddress to = new("bextiyar2901@gmail.com");
        //    //MailMessage message = new MailMessage(from, to);
        //    //message.Subject = "Test";
        //    //message.Body = "Brat uzurlu say narahat edirem Test Edirdim gorek Mail gondere bilecemmi";
        //    //smtp.Send(message);
        //    //return Ok("Alindi");
        //    service.SendAsync().Wait();
        //    return View();
        //}

    }
}
