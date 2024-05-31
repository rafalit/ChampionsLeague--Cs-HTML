using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using static WebApplication1.Controllers.UsersController;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("account/login")]
        public IActionResult Login()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "account", "login.html"), "text/html");
        }

        [HttpPost("account/login")]
        public IActionResult Login([FromForm] LoginModel model)
        {
            var user = _userService.Login(model.Username, model.Password);
            if (user == null)
            {
                // Możesz dodać komunikat o błędzie w login.html, aby wyświetlić użytkownikowi odpowiedni komunikat
                ModelState.AddModelError("", "Invalid username or password");
                return RedirectToAction("Login");
            }

            // Przekierowanie na stronę z wyborem zespołów po zalogowaniu
            return Redirect("/teams/chooseteam");
        }

        [HttpGet("account/register")]
        public IActionResult Register()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "account", "register.html"), "text/html");
        }

        [HttpPost("account/register")]
        public IActionResult Register([FromForm] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email
                };

                _userService.Register(user, model.Password);
                return Redirect("/account/login");
            }
            return Redirect("/account/register"); // Możesz dodać komunikat o błędzie w register.html
        }
    }
}
