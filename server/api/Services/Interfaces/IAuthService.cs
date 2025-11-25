using System.Security.Claims;
using api.Models;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using api.Models.Requests;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace api.Services;

public interface IAuthService
{
    AuthUserInfo Authenticate(LoginRequest request);
    Task<AuthUserInfo> Register(RegisterRequestDto request);
    AuthUserInfo? GetUserInfo(ClaimsPrincipal principal);
}