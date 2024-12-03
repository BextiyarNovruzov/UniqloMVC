using FrontToBackMvc.Enums;
using FrontToBackMvc.Models;
using FrontToBackMvc.ViewModels.Auths;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FrontToBackMvc.Controllers
{
    public class AccountController(UserManager<User> _userMeneger, SignInManager<User> _signinMeneger) : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserVM vm)
        {
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
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            return View();

        }

        public async Task<IActionResult> Login(LoginVM vm , string ReturnUrl)
        {
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
            if(!result.Succeeded)
            {

                if (result.IsNotAllowed)
                    ModelState.AddModelError("", "UserName or Password is wrong.");
                if (result.IsLockedOut)
                    ModelState.AddModelError("", "Wait Until" + user.LockoutEnd!.Value.ToString("yyyy-MM-dd:mm:ss"));
                return View();
            }
            if(ReturnUrl.IsNullOrEmpty())
            {
                return RedirectToAction("Index", "Home");
            }

            return LocalRedirect(ReturnUrl);

        }

        public async Task<IActionResult> Logout()
        {
            await _signinMeneger.SignOutAsync();
            return View(nameof(Login));
        }

    }
}
