using Application.Commands.Auth;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _mediator.Send(new AuthQuery
            {
                Email = request.Email,
                Senha = request.Senha,
                DeviceId = request.DeviceId
            });

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _mediator.Send(new RefreshTokenCommand
            {
                Token = request.Token,
                DeviceId = request.DeviceId
            });

            return Ok(response);
        }

        [HttpPost("frontend-login")]
        public async Task<IActionResult> FrontendLogin([FromBody] LoginRequest request)
        {
            var response = await _mediator.Send(new AuthQuery
            {
                Email = request.Email,
                Senha = request.Senha,
                DeviceId = request.DeviceId
            });


            SetRefreshTokenCookie(response.RefreshToken);

            return Ok(new
            {
                accessToken = response.Token,
                deviceId = request.DeviceId
            });
        }


        [HttpPost("frontend-refresh")]
        public async Task<IActionResult> FrontendRefresh([FromBody] RefreshTokenRequest request)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "No refresh token present" });

            var response = await _mediator.Send(new RefreshTokenCommand
            {
                Token = refreshToken,
                DeviceId = request.DeviceId
            });
            if ( response.Token == "a")
            {
                Response.Cookies.Append("refreshToken", "", new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(-1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                });
                throw new UnauthorizedAccessException("Refresh token inválido ou expirado.");
            }



            SetRefreshTokenCookie(response.RefreshToken);


            return Ok(new
            {
                accessToken = response.Token,
                deviceId = request.DeviceId
            });
        }



        private void SetRefreshTokenCookie(string token)
        {
            Response.Cookies.Append("refreshToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(30),
                Path = "/"
            });
        }

    }
}


public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    public string Token { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
}

