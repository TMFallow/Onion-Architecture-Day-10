using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using OA.Data;
using OA.Service;
using Onion_Architecture.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Onion_Architecture.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService userService;

        private IConfiguration configuration;

        public LoginController(IUserService userService, IConfiguration configuration)
        {
            this.userService = userService;
            this.configuration = configuration;
        }
        [HttpGet]
        public IActionResult LoginUser()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult LoginUser(UserViewModel model)
        {
            var user = Authenticate(model);
            if(user != null)
            {
                var token = Generate(user);
                //return Ok(token);
                return RedirectToAction("Index", "User");
            }
            return NotFound("User Not Found");
        }

        private object Generate(UserViewModel user)
        {
            var sercurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(sercurityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim("Password", user.Password),
            };
            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserViewModel? Authenticate(UserViewModel model)
        {
            bool user = userService.CheckUser(model.UserName, model.Password);
            if(user == true)
            {
                return model;
            }
            return null;
        }
    }
}
