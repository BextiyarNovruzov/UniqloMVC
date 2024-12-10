using FrontToBackMvc.Enums;
using FrontToBackMvc.Models;
using FrontToBackMvc.ViewModels.Auths;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Mail;

namespace FrontToBackMvc.Controllers
{
    public class AccountController(UserManager<User> _userMeneger, SignInManager<User> _signinMeneger) : Controller
    {
        bool isAuthonticate => User.Identity?.IsAuthenticated ?? false;

        public IActionResult Register()
        {
            if (isAuthonticate) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserVM vm)
        {
            if (isAuthonticate) return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid) return View();
            User user = new User
            {
                UserName = vm.UserName,
                Email = vm.Email,
                FullName = vm.FullName,
                ProfileImageUrl = "ProfilePhoto.img"
            };
            var result = await _userMeneger.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            var RoleResult = await _userMeneger.CreateAsync(user, nameof(Roles.User));
            if (!RoleResult.Succeeded)
            {
                foreach (var error in RoleResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            return View();

        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm, string? ReturnUrl = null)
        {
            if (isAuthonticate) return RedirectToAction("Index", "Home");
            if (!ModelState.IsValid) return View();
            User? user = null;
            if (vm.UserNameOrEmail.Contains('@'))
                user = await _userMeneger.FindByEmailAsync(vm.UserNameOrEmail);
            else
                user = await _userMeneger.FindByNameAsync(vm.UserNameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "UserName or Password is wrong.");
            }

            var result = await _signinMeneger.PasswordSignInAsync(user, vm.Password, vm.RememberMe, true);
            if (!result.Succeeded)
            {

                if (result.IsNotAllowed)
                    ModelState.AddModelError("", "UserName or Password is wrong.");
                if (result.IsLockedOut)
                    ModelState.AddModelError("", "Wait Until" + user.LockoutEnd!.Value.ToString("yyyy-MM-dd:mm:ss"));
                return View();
            }
            if (string.IsNullOrEmpty(ReturnUrl))
            {
                if (await _userMeneger.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Index", new { Controller = "Dashboard", Area = "Admin" });
                }
                return RedirectToAction("Index", "Home");
            }

            return LocalRedirect(ReturnUrl);

        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signinMeneger.SignOutAsync();
            return View(nameof(Login));
        }

        public async Task<IActionResult> Test()
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;  
            smtp.Credentials = new NetworkCredential("baxtiyarbn-bp215@code.edu.az", "hoff xmys zjfg enip");
            MailAddress from = new MailAddress("baxtiyarbn-bp215@code.edu.az", "Elvet Steakhouse");
            MailAddress to = new("bextiyar2901@gmail.com");
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Test";
            message.Body = "Brat uzurlu say narahat edirem Test Edirdim gorek Mail gondere bilecemmi";
            smtp.Send(message);
            return Ok("Alindi");
        }

    }
}
