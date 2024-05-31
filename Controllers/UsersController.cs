using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public ActionResult<User> Register([FromBody] RegisterModel model)
        {
            var user = new User
            {
                Username = model.Username,
                Email = model.Email
            };

            var createdUser = _userService.Register(user, model.Password);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.UserId }, createdUser);
        }

        [HttpPost("login")]
        public ActionResult<User> Login([FromBody] LoginModel model)
        {
            var user = _userService.Login(model.Username, model.Password);
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            return Ok(user);
        }

        [HttpGet("getAll")]
        public ActionResult<List<User>> GetAll()
        {
            return _userService.Get();
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetById(string id)
        {
            var user = _userService.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            // Przekieruj na stronę logowania
            return RedirectToAction("Login", "Users");
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            // Przekieruj na stronę rejestracji
            return RedirectToAction("Register", "Users");
        }

        public class RegisterModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }

        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
