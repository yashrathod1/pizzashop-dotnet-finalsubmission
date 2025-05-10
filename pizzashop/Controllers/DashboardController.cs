using Microsoft.AspNetCore.Mvc;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop.Controllers;

public class DashboardController : Controller
{
    private readonly IUserService _useService;

    public DashboardController(IUserService userService)
    {
        _useService = userService;
    }


    [HttpGet]
    public async Task<IActionResult> Index(string filter = "Current Month")
    {   
        ViewBag.ActiveNav = "Dashboard";
        var model = await _useService.GetDashboardDataAsync(filter);
        return View(model);
    }

    [HttpGet]
    public IActionResult Profile()
    {
        try
        {
            string? token = Request.Cookies["AuthToken"];
            string? email = _useService.ExtractEmailFromToken(token);

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Login");
            }

            UserTableViewModel? model = _useService.GetUserProfile(email);

            if (model == null)
            {
                return NotFound("User Not Found");
            }

            ViewBag.Email = email;
            return View(model);
        }
        catch
        {
            TempData["error"] = "An error occurred while fetching the profile.";
            return RedirectToAction("Login", "Login");
        }
    }


    [HttpPost]
    public IActionResult Profile(UserTableViewModel model)
    {
        try
        {
            string? token = Request.Cookies["AuthToken"];
            string? email = _useService.ExtractEmailFromToken(token);

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Login");
            }

            bool success = _useService.UpdateUserProfile(email, model);

            CookieOptions? coockieopt = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("Username", model.Username, coockieopt);
            Response.Cookies.Append("ProfileImgPath", string.IsNullOrEmpty(model.ProfileImagePath) ? "/images/Default_pfp.svg.png" : model.ProfileImagePath, coockieopt);

            if (!success)
            {
                return NotFound("User Not Found");
            }

            TempData["success"] = "Profile updated successfully.";
            return View(model);
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while updating the profile.";
            return RedirectToAction("Profile");

        }
    }


    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ChangePassword(ChangePasswordViewModel model)
    {
        try
        {
            string? token = Request.Cookies["AuthToken"];
            string? userEmail = _useService.ExtractEmailFromToken(token);

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Login");
            }

            string? result = _useService.ChangePassword(userEmail, model);

            if (result == "UserNotFound")
            {
                TempData["error"] = "User not found.";
                return View(model);
            }

            if (result == "IncorrectPassword")
            {
                TempData["error"] = "Current password is incorrect.";
                return View(model);
            }

            TempData["success"] = "Password updated successfully.";
            return RedirectToAction("Login", "Login");
        }
        catch (Exception)
        {
            TempData["error"] = "An unexpected error occurred.";
            return View(model);
        }
    }


    public IActionResult Logout()
    {
        if (Request.Cookies["UserEmail"] != null)
        {
            Response.Cookies.Delete("UserEmail");
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("Username");
            Response.Cookies.Delete("ProfileImgPath");

        }
        return RedirectToAction("Login", "Auth");
    }




}
