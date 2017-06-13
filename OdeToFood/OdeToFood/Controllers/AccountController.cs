using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OdeToFood.Models;
using OdeToFood.Services;
using OdeToFood.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdeToFood.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IEmailService _emailService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User() { UserName = model.UserName, FirstName=model.FirstName, LastName=model.LastName, Email=model.Email,AlternateEmail=model.AlternateEmail,PhoneNumber=model.PhoneNumber,Organisation=model.Organisation,Position=model.Position};

               var createResult=  await _userManager.CreateAsync(user, model.Password);
                if (createResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach(var error in createResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View();
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginviewModel model)
        {
            if (ModelState.IsValid)
            {
                var loginResult = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                if (loginResult.Succeeded)
                {
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }               
                }
            }
            ModelState.AddModelError("", "Could not login");
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                User currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                
                var changePasswordResult = await _userManager.ChangePasswordAsync(currentUser, model.Password, model.NewPassword);
                if (changePasswordResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            
            ModelState.AddModelError("", "Could not login");
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                User currentUser = await _userManager.FindByEmailAsync(model.Email);

                if (currentUser != null &&  _userManager.IsEmailConfirmedAsync(currentUser).Result)
                {
                    string token = await _userManager.GeneratePasswordResetTokenAsync(currentUser);
                    string callbackUrl = Url.Action("ResetPassword", "Account",new { token = token}, protocol: HttpContext.Request.Scheme);

                    var response = await this._emailService.SendAsync(
                         to: currentUser.Email,
                         subject: "Reset password",
                         body: $"<strong>Reset your password by clicking this link <br/> {callbackUrl} </strong>");
                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                }
            }
            ModelState.AddModelError("", "Could not send email");
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ResetPassword
                (ResetPasswordViewModel obj)
        {
            User user = _userManager.
                         FindByNameAsync(obj.UserName).Result;

            IdentityResult result = _userManager.ResetPasswordAsync
                      (user, obj.Token, obj.Password).Result;
            if (result.Succeeded)
            {
                ModelState.AddModelError("", "Password reset successfull!");
                return View(obj);
            }
            else
            {
                ModelState.AddModelError("", "Password reset not successfull!");
                return View(obj);
            }
        }


    }
}
