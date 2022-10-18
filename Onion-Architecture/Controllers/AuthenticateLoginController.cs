using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Service;
using Onion_Architecture.Models;
using System.Security.Claims;

namespace Onion_Architecture.Controllers
{
    public class AuthenticateLoginController : Controller
    {
        private readonly IUserService userService;

        private readonly ITokenService tokenService;

        private readonly IConfiguration configuration;

        public AuthenticateLoginController(IUserService userService, IConfiguration configuration, ITokenService tokenService)
        {
            this.userService = userService;
            this.configuration = configuration;
            this.tokenService = tokenService;
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
            if (model is null)
            {
                return BadRequest("Invalid Client Request");
            }
            var user = userService.CheckUser(model.UserName, model.Password);
            if (user == false)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, model.UserName),
                new Claim(ClaimTypes.Role, "User")
            };

            var accessToken = tokenService.GenerateAccessToken(claims);
            var refreshToken = tokenService.GenerateRefreshToken();

            model.RefreshToken = refreshToken;
            model.RefreshTokenExpireTime = DateTime.Now.AddSeconds(10);

            userService.SaveChanges();

            return Ok(new AuthenticatedRespone
            {
                Token = accessToken,
                RefreshToken = refreshToken
            });
        }
    }
}
