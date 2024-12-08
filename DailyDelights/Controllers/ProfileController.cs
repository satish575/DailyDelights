using Microsoft.AspNetCore.Mvc;
using DailyDelights.ViewModels;
using Microsoft.AspNetCore.Identity;
using DailyDelights.Models;
using DailyDelights.DatabaseContext;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace DailyDelights.Controllers
{
   [Authorize(Roles ="Customer")]
    public class ProfileController : Controller
    {
        // GET: ProfileController
      private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;  

    public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }
 
    public async Task<IActionResult> GetProfile()
    {
        var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid userid;
        Guid.TryParse(userId,out userid);
       

        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); 
        }

        var userProfile = await _context.Users
            .Where(u => u.Id == userid)
            .Select(u => new UserProfile
            {
                Id = u.Id,
                Gender = u.Gender,
                DOB = u.DOB,
                UserName = u.UserName,
                NormalizedUserName = u.NormalizedUserName,
                Email = u.Email,
                NormalizedEmail = u.NormalizedEmail,
                EmailConfirmed = u.EmailConfirmed,
                PasswordHash = u.PasswordHash,
                ImagePath=u.ImageUrl
            })
            .FirstOrDefaultAsync();

        if (userProfile == null)
        {
            return NotFound();
        }

        return View(userProfile);
    }

    [HttpGet]
public async Task<IActionResult> EditProfile()
{
    var userId = _userManager.GetUserId(User); // Get the logged-in user's ID
    if (userId == null)
    {
        return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
    }

    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
        return NotFound();
    }

    // Map user data to the UserProfileEdit model
    var userProfile = new UserProfileEdit
    {
        UserName = user.UserName,
        Email = user.Email,
        Gender = user.Gender,
        DOB = user.DOB,
        ImagePath = user.ImageUrl
    };

    return View(userProfile);
}

[HttpPost]
public async Task<IActionResult> EditProfile(UserProfileEdit model)
{
    if (ModelState.IsValid)
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        // Update user fields
        user.UserName = model.UserName;
        user.Email = model.Email;
        user.Gender = model.Gender;
        user.DOB = model.DOB;

        // Handle image upload if provided
        if (model.image != null && model.image.Length > 0)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", model.image.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.image.CopyToAsync(stream);
            }

            user.ImageUrl = $"{model.image.FileName}";
        }

        // Update the user in the database
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction("GetProfile");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }

    return View(model);
}


public IActionResult ChangePassword(){
    return View(new ChangePasswordViewModel());
}

[HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isPasswordChanged = ChangeUserPassword(model.CurrentPassword, model.NewPassword);

            if (isPasswordChanged)
            {
                TempData["SuccessMessage"] = "Password changed successfully.";
                return RedirectToAction("GetProfile", "Profile");
            }

            ModelState.AddModelError(string.Empty, "Failed to change password. Ensure the current password is correct.");
            return View(model);
        }

        private bool ChangeUserPassword(string currentPassword, string newPassword)
        {
            var user = _userManager.GetUserAsync(User).Result; // Get the currently logged-in user

            if (user == null)
            {
                return false; // User is not logged in
            }

            // Verify the current password and update it with the new password
            var result = _userManager.ChangePasswordAsync(user, currentPassword, newPassword).Result;

            return result.Succeeded;
        }


    }
}
