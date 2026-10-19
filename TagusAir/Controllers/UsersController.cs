using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TagusAir.Data.Entities;
using TagusAir.Helpers;
using TagusAir.Models;

namespace TagusAir.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;

        public UsersController(
            IUserHelper userHelper,
            IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
        }
        public async Task<IActionResult> Index()
        {
            var model = new UserListViewModel
            {
                Employees = await _userHelper.GetUsersInRoleAsync("Employee"),
                Customers = await _userHelper.GetUsersInRoleAsync("Customer"),

            };

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userExists = await _userHelper.GetUserByEmailAsync(viewModel.Email);

                if (userExists != null)
                {
                    ModelState.AddModelError(string.Empty, "There is already an account registered with this email.");
                    return View(viewModel);
                }

                User createdUser = new User
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Email = viewModel.Email,
                    UserName = viewModel.Email,
                    PhoneNumber = viewModel.PhoneNumber,
                };

                var generatedPassword = Guid.NewGuid().ToString();

                var result = await _userHelper.AddUserAsync(createdUser, generatedPassword);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Could not create the user account.");
                    return View(viewModel);
                }

                await _userHelper.AddUserToRoleAsync(createdUser, viewModel.Role);

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(createdUser);

                var link = this.Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken },
                    protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendEmail(
                    viewModel.Email,
                    "Welcome to TagusAir",
                    $"<h1>Welcome to TagusAir</h1>" +
                    $"An account has been created for you with the role of {viewModel.Role}.</br></br>" +
                    $"To set your password and activate your account, click in this link:</br></br>" +
                    $"<a href = \"{link}\">Set Password</a>");

                if (response.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "The user was created but the email could not be sent.");
                return View(viewModel);
            }

            return View(viewModel);
        }
    }
}
