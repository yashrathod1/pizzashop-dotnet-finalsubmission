using Microsoft.AspNetCore.Mvc;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;
namespace pizzashop.Controllers;

public class AuthController : Controller
{

    private readonly IUserService _useService;
    private readonly IEmailSender _emailSender;
    private readonly IJwtService _jwtService;

    public AuthController(IUserService userService, IEmailSender emailSender, IJwtService jwtService)
    {
        _useService = userService;
        _emailSender = emailSender;
        _jwtService = jwtService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        try
        {
            string? req_cookie = Request.Cookies["UserEmail"];
            if (!string.IsNullOrEmpty(req_cookie))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }
        catch
        {
            return RedirectToAction("AccessDenied", "Auth");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = _useService.AuthenicateUser(model.Email, model.Password);
                if (user == null)
                {
                    TempData["error"] = "Invalid Email or Password";
                    return View(model);
                }

                CookieOptions? coockieopt = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };

                if (model.RememberMe)
                {
                    coockieopt.Expires = DateTime.UtcNow.AddDays(30);
                    Response.Cookies.Append("UserEmail", user.Email, coockieopt);
                }

                Response.Cookies.Append("Username", user.Username, coockieopt);
                Response.Cookies.Append("ProfileImgPath", string.IsNullOrEmpty(user.Profileimagepath) ? "/images/Default_pfp.svg.png" : user.Profileimagepath, coockieopt);

                string token = await _useService.GenerateJwttoken(user.Email, user.Roleid);

                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddHours(24)
                });

                TempData["success"] = "Login Successful";

                return RedirectToAction("Index", "Dashboard");
            }

            return View(model);
        }
        catch
        {
            TempData["error"] = "An error occurred during login";
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult ForgotPassword(string email)
    {
        try
        {
            if (!string.IsNullOrEmpty(email))
            {
                ViewData["Email"] = email;
            }
            else
            {
                ViewData["Email"] = "";
            }
            return View();
        }
        catch
        {
            TempData["error"] = "An error occurred while processing your request";
            return View();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ForgotPassword(ForgotViewModel objUser)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(objUser);
            }

            var user = _useService.GetUserByEmail(objUser.Email);

            if (user == null)
            {
                TempData["error"] = "User with this email does not exist";
                return View(objUser);
            }

            string resetToken = _useService.GeneratePasswordResetToken(user.Email);
            string filePath = @"C:/Users/pci100/Desktop/3tiertryerror/pizzashop/Template/EmailTemplate.html";
            string emailBody = System.IO.File.ReadAllText(filePath);

            string? resetLink = Url.Action("ResetPassword", "Auth", new { token = resetToken }, Request.Scheme);
            emailBody = emailBody.Replace("{ResetLink}", resetLink);

            string subject = "Reset Password";
            _emailSender.SendEmailAsync(objUser.Email, subject, emailBody);

            TempData["success"] = "Password reset instructions have been sent to your email.";

            return View(objUser);
        }
        catch
        {
            TempData["error"] = "An error occurred while processing your request";
            return View(objUser);
        }
    }


    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["error"] = "Invalid reset link";
                return RedirectToAction("ForgotPassword");
            }

            ResetPasswordViewModel? model = new ResetPasswordViewModel { Token = token };
            return View(model);
        }
        catch
        {
            TempData["error"] = "An error occurred while processing your request";
            return RedirectToAction("ForgotPassword");
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ResetPassword(ResetPasswordViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_useService.ResetPassword(model.Token, model.NewPassword, model.ConfirmPassword, out string message))
            {
                TempData["success"] = "Password Successfully Reset";
                return RedirectToAction("Login", "Auth");
            }

            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }
        catch
        {
            TempData["error"] = "An error occurred while resetting your password. Please try again.";
            return View(model);
        }
    }


    [CustomAuthorize("RoleAndPermission", "CanView")]
    [HttpGet]
    public IActionResult Roles()
    {
        try
        {
            ViewBag.ActiveNav = "Role";
            return View();
        }
        catch
        {
            TempData["error"] = "We encountered an issue while loading the roles.";
            return RedirectToAction("Roles");
        }
    }


    [CustomAuthorize("RoleAndPermission", "CanView")]
    [HttpGet]
    public async Task<IActionResult> Permissions(string role)
    {
        try
        {
            ViewBag.ActiveNav = "Role";
            ViewBag.SelectedRole = role;

            List<RolePermissionViewModel>? permissions = await _useService.GetPermissionsByRoleAsync(role);

            if (permissions == null || !permissions.Any())
            {
                TempData["error"] = "No permissions found for the selected role.";
                return View(new List<RolePermissionViewModel>());
            }

            return View(permissions);
        }
        catch (Exception)
        {
            TempData["error"] = "An error occurred while fetching permissions.";
            return RedirectToAction("Roles");
        }
    }



    [HttpPost]
    public async Task<IActionResult> UpdatePermissions([FromBody] List<RolePermissionViewModel> updatedPermissions)
    {
        try
        {
            if (updatedPermissions == null || !updatedPermissions.Any())
            {
                return Json(new { success = false, message = "No permissions received." });
            }

            bool result = await _useService.UpdateRolePermissionsAsync(updatedPermissions);

            if (result)
            {
                return Json(new { success = true, message = "Permissions updated successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Failed to update permissions." });
            }
        }
        catch
        {
            return Json(new { success = false, message = "An error occurred while updating permissions. Please try again later." });
        }
    }

    public IActionResult AccessDenied()
    {
        return View();
    }


}
