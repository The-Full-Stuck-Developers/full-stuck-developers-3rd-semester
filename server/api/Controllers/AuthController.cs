using api.Models;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Requests.Auth;
using api.Models.Dtos.Responses;
using api.Models.Requests;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/auth")]
public class AuthController(
    IAuthService authService,
    ITokenService tokenService,
    ILogger<AuthController> logger)
    : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost(nameof(Login))]
    [AllowAnonymous]
    public async Task<LoginResponse> Login([FromBody] LoginRequestDto request)
    {
        var userInfo = _authService.Authenticate(request);
        var token = _tokenService.CreateToken(userInfo);
        return new LoginResponse(token);
    }

    [HttpPost(nameof(Register))]
    [AllowAnonymous]
    public async Task<RegisterResponse> Register([FromBody] RegisterRequestDto request)
    {
        var userInfo = await _authService.Register(request);
        return new RegisterResponse(Name: userInfo.Name);
    }

   /* [HttpPost(nameof(WhoAmI))]
    public async Task<JwtClaims> WhoAmI()
    {
        var jwtClaims = await authService.VerifyAndDecodeToken(Request.Headers.Authorization.FirstOrDefault());
        return jwtClaims;
    }*/
    [HttpPost(nameof(ForgotPassword))]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
    {
        try
        {
            await _authService.SendPasswordResetEmail(dto.Email);
            return Ok(new { message = "If an account exists, you'll receive an email shortly" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in forgot password");
            return Ok(new { message = "If an account exists, you'll receive an email shortly" });
        }
    }
    [HttpPost(nameof(ResetPassword))]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
    {
        try
        {
            await _authService.ResetPassword(dto);
            return Ok(new { message = "Password reset successful" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password");
            return BadRequest(new { message = "Invalid or expired reset token" });
        }
    }
   [HttpPost(nameof(Logout))]
   public async Task<IResult> Logout()
   {
       throw new NotImplementedException();
   }
   
   [HttpGet(nameof(UserInfo))]
   public async Task<AuthUserInfo> UserInfo()
   {
       return _authService.GetUserInfo(User);
   }
}