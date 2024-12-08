using DailyDelights.Models;
using DailyDelights.Utils;
using DailyDelights.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DailyDelights.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signinManager;

        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager,RoleManager<ApplicationRole> roleManager,SignInManager<ApplicationUser> signInManager,IEmailSender emailSender){
            this._userManager=userManager;
            this._roleManager=roleManager;
            this._signinManager=signInManager;
            this._emailSender=emailSender;
        }
        // GET: AccountController

        [HttpGet]
        public ActionResult Login([FromQuery] string ReturnUrl)
        {
            ViewData["returnUrl"]=ReturnUrl;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel loginViewModel){
            if(!ModelState.IsValid){
             return View();
            }
            var user=await _userManager.FindByNameAsync(loginViewModel.Username);
            if(user is not null){
                var PasswordCheck=await _userManager.CheckPasswordAsync(user,loginViewModel.Password);
                if(PasswordCheck){
                    if(!await _userManager.IsEmailConfirmedAsync(user)){
                        ViewBag.EmailConfirm="Your email is not confirmed";
                        return View();
                    }
                   await _signinManager.SignInAsync(user,isPersistent:false);
                   if(!String.IsNullOrEmpty(loginViewModel.ReturnUrl) && Url.IsLocalUrl(loginViewModel.ReturnUrl)){
                     return Redirect(loginViewModel.ReturnUrl);
                   }
                   if((await _userManager.GetRolesAsync(user)).Contains("Admin")){
                    return RedirectToAction("Index","Admin");
                   }
                   return RedirectToAction("Index","Home");
                   
                }else{
                    ModelState.AddModelError("password","Invalid credentials");
                }
            }else{
                ModelState.AddModelError("username","User not found");
            }
            return View();
            
        }

        [HttpGet]
        public ActionResult Register(){
            return View();
        }
      




[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
{
    if (ModelState.IsValid)
    {
        // Create a new user
        var user = new ApplicationUser
        {
            UserName = registerViewModel.Username, // Use email as username for simplicity
            Email = registerViewModel.Email,
            Gender = registerViewModel.Gender,
            DOB = registerViewModel.DOB
        };

        var result = await _userManager.CreateAsync(user, registerViewModel.Password);

        if (result.Succeeded)
        {
            // Check if the role exists, create if necessary
            if (!await _roleManager.RoleExistsAsync(registerViewModel.Role))
            {
                await _roleManager.CreateAsync(new ApplicationRole
                {
                    Name = registerViewModel.Role
                });
            }

            // Assign the role to the user
            await _userManager.AddToRoleAsync(user, registerViewModel.Role);

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(
                "ConfirmEmail", 
                "Account", 
                new { userId = user.Id, token }, 
                Request.Scheme
            );

            // Send email with the confirmation link
            var emailSubject = "Confirm your email";
            var emailBody = $"Please confirm your account by clicking <a href='{confirmationLink}'>here</a>.";
            await _emailSender.SendEmailAsync(registerViewModel.Email, emailSubject, emailBody);

            ViewBag.Message = "Registration successful. Please check your email to confirm your account.";
            return View();
        }

        // Add errors to the model state
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    return View(registerViewModel);
}




// confirm email

[HttpGet]
public async Task<IActionResult> ConfirmEmail(string userId, string token)
{
    if (userId == null || token == null)
    {
        return RedirectToAction("Error", "Home");
    }

    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
        return RedirectToAction("Error", "Home");
    }

    var result = await _userManager.ConfirmEmailAsync(user, token);
    if (result.Succeeded)
    {
        ViewBag.Message = "Email confirmed successfully.";
        return View();
    }

    ViewBag.Message = "Email confirmation failed.";
    return View();
}




        public IActionResult AccessDenied(){
            return View();
        }


        public async Task<IActionResult> Logout(){
            await _signinManager.SignOutAsync();
            return RedirectToAction("Login","Account");
        }



        #region passwordreset
        [HttpGet]
public IActionResult ForgotPassword()
{
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
{
    if (ModelState.IsValid)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.IsEmailConfirmedAsync(user))
        {
            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Account", 
                new { email = model.Email, token }, Request.Scheme);

            // Send email
            var subject = "Reset Your Password";
            var body = $"Reset your password by clicking <a href='{resetLink}'>here</a>.";
            await _emailSender.SendEmailAsync(model.Email, subject, body);
        }

        // For security, don't reveal if the email is not registered
        ViewBag.Message = "If the email is registered, you will receive a reset link.";
        return View("ForgotPasswordConfirmation");
    }

    return View(model);
}

[HttpGet]
public IActionResult ResetPassword(string email, string token)
{
    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
    {
        return BadRequest("Invalid password reset token.");
    }

    var model = new ResetPasswordViewModel { Email = email, Token = token };
    return View(model);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }

    var user = await _userManager.FindByEmailAsync(model.Email);
    if (user == null)
    {
        // For security, don't reveal if the user does not exist
        ViewBag.Message = "Password has been reset.";
        return View("ResetPasswordConfirmation");
    }

    // Reset the password
    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
    if (result.Succeeded)
    {
        ViewBag.Message = "Password has been reset successfully.";
        return View("ResetPasswordConfirmation");
    }

    foreach (var error in result.Errors)
    {
        ModelState.AddModelError(string.Empty, error.Description);
    }

    return View(model);
}

      #endregion
    }
}
