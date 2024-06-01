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
            return RedirectToAction("ChooseTeam", "Teams");
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
                return RedirectToAction("Login");
            }

            // Jeśli ModelState.IsValid nie jest prawdziwe, dodaj błędy do ModelState
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }
            }

            // Przekieruj użytkownika z powrotem do formularza rejestracji z błędami
            return View("~/wwwroot/account/register.html", model);
        }
    }
}