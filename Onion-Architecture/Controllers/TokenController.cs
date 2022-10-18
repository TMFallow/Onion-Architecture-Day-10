using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using OA.Data;
using OA.Service;
using Onion_Architecture.Models;

namespace Onion_Architecture.Controllers
{
    public class TokenController : Controller
    {
        private readonly ITokenService tokenService;

        private readonly IUserService userService;

        private readonly IUserInfoService userInfoService;

        public TokenController(ITokenService tokenService, IUserService userService, IUserInfoService userInfoService)
        {
            this.tokenService = tokenService;
            this.userService = userService;
            this.userInfoService = userInfoService;
        }

        [HttpPost]
        [Route("Request")]
        public IActionResult Refresh(TokenAPIModel tokenAPIModel)
        {
            if(tokenAPIModel is null)
            {
                return BadRequest("Invalid Client Request");
            }
            var accessToken = tokenAPIModel.AccessToken;

            var refreshToken = tokenAPIModel.RefreshToken;

            var principal = tokenService.GetPrincipalFromExpiredToken(accessToken);

            var username = principal.Identity.Name;

            var user = userService.GetUserByUsername(username);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpireTime <= DateTime.Now)
                return BadRequest("Invalid Client Request");

            var newAccessToken = tokenService.GenerateAccessToken(principal.Claims);

            var newRefreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;

            userService.SaveChanges();

            return Ok(new AuthenticatedRespone
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
            });
        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;
            var user = userService.GetUserByUsername(username);
            if (user == null) return BadRequest();
            user.RefreshToken = null;
            userService.SaveChanges();
            return NoContent();
        }
    }
}
