using System.Security.Claims;
using api.Etc;
using api.Mappers;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using api.Security;
using api.Services;
using dataccess.Entities;
using dataccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace api.Services;


public class AuthService(
        ILogger<AuthService> _logger,
        IPasswordHasher<User> PasswordHasher,
        IRepository<User> _userRepository
    ) : IAuthService
    {
        public AuthUserInfo Authenticate(LoginRequest request)
        {
            // Null checks
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning("Authentication failed: email or password is empty");
                throw new AuthenticationError();
            }

            var user = _userRepository.Query()
                .FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
            {
                _logger.LogWarning("Authentication failed: user with email {Email} not found", request.Email);
                throw new AuthenticationError();
            }

            var result = PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result == PasswordVerificationResult.Success)
            {
                return new AuthUserInfo(user.Id, user.Name, user.IsAdmin);
            }
    
            _logger.LogWarning("Authentication failed: invalid password for user {Email}", request.Email);
            throw new AuthenticationError();
        }

        public async Task<AuthUserInfo> Register(RegisterRequestDto request)
        {
            // Check if email already exists
             var existingUser = _userRepository.Query()
                 .Any(u => u.Email == request.Email);

             if (existingUser)
             {
                 _logger.LogWarning("Registration failed: email {Email} already in use", request.Email);
                 throw new AuthenticationError();
             }
             var user = new User
             {
                 Email = request.Email,
                 Name = request.Name,
                 IsAdmin = false,
                 // set other fields as needed
             };

             user.PasswordHash = PasswordHasher.HashPassword(user, request.Password);

             await _userRepository.AddAsync(user);


             return new AuthUserInfo(user.Id, user.Name, user.IsAdmin);

        }
        public AuthUserInfo? GetUserInfo(ClaimsPrincipal principal)
        {
            var userId = principal.GetUserId();
            return _userRepository
                .Query()
                .SingleOrDefault(user => user.Id == userId)
                ?.ToDto();
        }
    }