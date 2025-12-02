using api.Models;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using api.Models.Requests;
using api.Services;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/auth")]
public class AuthController(IAuthService authService, ITokenService tokenService) : ControllerBase
{
    [HttpPost(nameof(Login))]
    [AllowAnonymous]
    public async Task<LoginResponse> Login([FromBody] LoginRequest request)
    {
        var userInfo = authService.Authenticate(request);
        var token = tokenService.CreateToken(userInfo);
        return new LoginResponse(token);
    }

    [HttpPost(nameof(Register))]
    [AllowAnonymous]
    public async Task<RegisterResponse> Register([FromBody] RegisterRequestDto request)
    {
        var userInfo = await authService.Register(request);
        return new RegisterResponse(Name: userInfo.Name);
    }

   /* [HttpPost(nameof(WhoAmI))]
    public async Task<JwtClaims> WhoAmI()
    {
        var jwtClaims = await authService.VerifyAndDecodeToken(Request.Headers.Authorization.FirstOrDefault());
        return jwtClaims;
    }
    [HttpPost(nameof(ForgotPassword))]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        await authService.ForgotPassword(dto);
        return Ok("If the email exists, a reset link was sent.");
    }
    [HttpPost(nameof(ResetPassword))]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        await authService.ResetPassword(dto);
        return Ok("Password updated!");
    }*/
   [HttpPost(nameof(Logout))]
   public async Task<IResult> Logout()
   {
       throw new NotImplementedException();
   }
   
   [HttpGet(nameof(UserInfo))]
   public async Task<AuthUserInfo> UserInfo()
   {
       return authService.GetUserInfo(User);
   }
}