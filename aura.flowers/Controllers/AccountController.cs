using System.Threading.Tasks;
using aura.flowers.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using website.core.Models.Auth;
using website.services.Interfaces;

namespace aura.flowers.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthService _authService;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
        }

        /// <summary>
        /// GET: Account/Index
        /// Account management index page.
        /// </summary>
        /// <returns>Show index page.</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _authService.GetAllUsers());
        }

        /// <summary>
        /// GET: Account/Register
        /// New account register page.
        /// </summary>
        /// <returns>Register page view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return RedirectToAction("Register", "Account");
        }

        /// <summary>
        /// POST: Account/Register
        /// Add a new user from register view.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Redirect on account main page or showing validation errors.</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            User user = new User
            {
                Email = model.Email,
                UserName = model.Email
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Account");
            }

            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        /// <summary>
        /// GET: Account/Login
        /// Login view for sign in.
        /// </summary>
        /// <returns>Show login form.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        ///POST: Account/Login
        /// Sign in action.
        /// </summary>
        /// <param name="model">Login view model object.</param>
        /// <returns>Redirect on Hangfire dashboard or login page if error.</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Login", model);

            Microsoft.AspNetCore.Identity.SignInResult result =
                await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Index", "Admin");
            }

            ModelState.AddModelError("", "Incorrect login or password!");

            return View("Login", model);
        }

        /// <summary>
        /// GET: Account/ChangePassword
        /// </summary>
        /// <param name="id">User id string.</param>
        /// <returns>Change password view.</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string id)
        {
            User user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                Id = user.Id,
                Email = user.Email
            };

            return View(model);
        }

        /// <summary>
        /// POST: Account/ChangePassword
        /// </summary>
        /// <param name="model">Change password model object.</param>
        /// <returns>Account management index page or error page.</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);

                if (user != null)
                {
                    IPasswordValidator<User> passwordValidator =
                        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                    IPasswordHasher<User> passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;

                    if (passwordValidator != null)
                    {
                        IdentityResult result =
                            await passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);

                        if (result.Succeeded)
                        {
                            if (passwordHasher != null)
                                user.PasswordHash = passwordHasher.HashPassword(user, model.NewPassword);

                            await _userManager.UpdateAsync(user);
                            return RedirectToAction("Index", "Account");
                        }

                        foreach (IdentityError error in result.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                    ModelState.AddModelError(string.Empty, "User not found.");
            }
            return View(model);
        }

        /// <summary>
        /// POST: Account/Remove
        /// Remove user from unitronics download system.
        /// </summary>
        /// <param name="id">User id string.</param>
        /// <returns>Account magement page or error page.</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Remove(string id)
        {
            User user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            IdentityResult result = await _userManager.DeleteAsync(user);

            return result.Succeeded ?
                RedirectToAction("Index", "Account") :
                RedirectToAction("Error", "Home");
        }

        /// <summary>
        /// GET: Account/LogOff
        /// Log off.
        /// </summary>
        /// <returns>Redirect on login page.</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}