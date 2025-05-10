using Microsoft.AspNetCore.Mvc;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;
using pizzashop_service.Implementation;
using pizzashop_service.Interface;

namespace pizzashop.Controllers;


public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IEmailSender _emailSender;

    public UsersController(IUserService userService, IEmailSender emailSender)
    {
        _userService = userService;
        _emailSender = emailSender;
    }

    public async Task<IActionResult> UsersList()
    {
        ViewBag.ActiveNav = "Users";
        RolePermissionViewModel? permission = await PermissionHelper.GetPermissionsAsync(HttpContext, "Users");

        ViewBag.Permissions = permission;
        return View();
    }
    public async Task<IActionResult> GetUsers(int pageNumber = 1, int pageSize = 5, string sortBy = "Name", string sortOrder = "asc", string searchTerm = "")
    {
        try
        {
            RolePermissionViewModel? permission = await PermissionHelper.GetPermissionsAsync(HttpContext, "Users");
            ViewBag.Permissions = permission;
            PagedResult<UserTableViewModel>? pagedItems = await _userService.GetUsersAsync(pageNumber, pageSize, sortBy, sortOrder, searchTerm);
            return PartialView("_UsersList", pagedItems);
        }
        catch
        {
            return Json(new { success = false, message = "An error occurred while fetching the users. Please try again later." });
        }

    }

    [CustomAuthorize("Users", "CanDelete")]
    [HttpGet("Delete/{id}")]
    public IActionResult DeleteUser(int id)
    {
        try
        {
            bool isDeleted = _userService.DeleteUser(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            TempData["success"] = "User deleted successfully";
            return RedirectToAction("UsersList");
        }
        catch
        {
            TempData["error"] = "An error occurred while deleting the user.";
            return RedirectToAction("UsersList");
        }
    }


    [HttpGet("AddUser")]
    public IActionResult AddUser()
    {
        ViewBag.ActiveNav = "Users";
        ViewBag.Roles = _userService.GetRoles();
        return View();
    }

    [CustomAuthorize("Users", "CanAddEdit")]
    [HttpPost("AddUser")]
    public async Task<IActionResult> AddUser([FromForm] AddUserViewModel model)
    {
        try
        {
            ViewBag.Roles = _userService.GetRoles();

            var emailverify = _userService.GetUserByEmail(model.Email);
            if (emailverify != null)
            {
                TempData["error"] = "Account already exists with Email";
                return View(model);
            }

            var usernameverify = _userService.GetUserByUsername(model.Username);
            if (usernameverify != null)
            {
                TempData["error"] = "Account already exists with username";
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please Enter Valid Data" });
            }

            bool isAdded = await _userService.AddUser(model);
            if (isAdded)
            {
                string filePath = @"D:/3tierpizzashop/pizzashop/Template/AddNewUserEmailTemplate.html";
                string emailBody = System.IO.File.ReadAllText(filePath);
                emailBody = emailBody.Replace("{Email}", model.Email);
                emailBody = emailBody.Replace("{Password}", model.Password);

                string subject = "Your Login Details";
                await _emailSender.SendEmailAsync(model.Email, subject, emailBody);

                TempData["success"] = "User Successfully Added";
                return RedirectToAction("UsersList", "Users");
            }

            ModelState.AddModelError("", "Failed to add user. Role may not exist.");
            return View(model);
        }
        catch
        {
            TempData["error"] = "An error occurred while adding the user";
            return View(model);
        }
    }


    [HttpGet("EditUser/{id}")]
    public IActionResult EditUser(int id)
    {
        try
        {
            ViewBag.ActiveNav = "Users";
            EditUserViewModel? model = _userService.GetUserForEdit(id);
            if (model == null)
            {
                return NotFound();
            }

            ViewBag.Roles = _userService.GetRoles();
            return View(model);
        }
        catch
        {
            TempData["error"] = "An error occurred while fetching user details. Please try again later.";
            return RedirectToAction("UsersList");
        }
    }


    [CustomAuthorize("Users", "CanAddEdit")]
    [HttpPost("EditUser/{id}")]
    public async Task<IActionResult> EditUser([FromForm] EditUserViewModel model, int id)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _userService.GetRoles();
                return View(model);
            }

            bool isUpdated = await _userService.EditUser(id, model);
            if (!isUpdated)
            {
                TempData["error"] = "No Change Found Please Update the User";
                ViewBag.Roles = _userService.GetRoles();
                return View(model);
            }
            TempData["success"] = "User Successfully Edited";
            return RedirectToAction("UsersList");
        }
        catch
        {
            TempData["error"] = "An error occurred while updating the user.";
            ViewBag.Roles = _userService.GetRoles();
            return View(model);
        }
    }

}
