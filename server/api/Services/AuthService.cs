using System.Security.Claims;
using System.Security.Cryptography;
using api.Etc;
using api.Mappers;
using api.Models;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Requests.Auth;
using api.Models.Dtos.Responses;
using api.Models.Requests;
using api.Security;
using api.Services;
using dataccess.Entities;
using dataccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace api.Services;


public class AuthService(
    ILogger<AuthService> logger,
    IPasswordHasher<User> passwordHasher,
    IRepository<User> userRepository,
    IEmailService emailService,
    AppOptions appOptions)
    : IAuthService
{
    public AuthUserInfo Authenticate(LoginRequestDto request)
        {
            // Null checks
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                logger.LogWarning("Authentication failed: email or password is empty");
                throw new AuthenticationError();
            }

            var user = userRepository.Query()
                .FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
            {
                logger.LogWarning("Authentication failed: user with email {Email} not found", request.Email);
                throw new AuthenticationError();
            }

            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result == PasswordVerificationResult.Success)
            {
                return new AuthUserInfo(user.Id, user.Name, user.IsAdmin);
            }
    
            logger.LogWarning("Authentication failed: invalid password for user {Email}", request.Email);
            throw new AuthenticationError();
        }

        public async Task<AuthUserInfo> Register(RegisterRequestDto request)
        {
            // Check if email already exists
             var existingUser = userRepository.Query()
                 .Any(u => u.Email == request.Email);

             if (existingUser)
             {
                 logger.LogWarning("Registration failed: email {Email} already in use", request.Email);
                 throw new AuthenticationError();
             }
             var user = new User
             {
                 Email = request.Email,
                 Name = request.Name,
                 IsAdmin = false,
                 CreatedAt = DateTime.UtcNow,
                 UpdatedAt = DateTime.UtcNow
             };

             user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

             await userRepository.AddAsync(user);


             return new AuthUserInfo(user.Id, user.Name, user.IsAdmin);

        }
        public AuthUserInfo? GetUserInfo(ClaimsPrincipal principal)
        {
            var userId = principal.GetUserId();
            return userRepository
                .Query()
                .SingleOrDefault(user => user.Id == userId)
                ?.ToDto();
        }

        public async Task SendPasswordResetEmail(string email)
        {
            var user = await userRepository.Query()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                logger.LogWarning("Password reset failed: user with email {Email} not found", email);
                return;
            }
            // Generate reset token (valid for 1 hour)
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var expiry = DateTime.UtcNow.AddHours(1);
            
            // Store token in database (you'll need to add these fields to User entity)
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = expiry;
            await userRepository.UpdateAsync(user);
                
            
            // Send email
            var resetLink = $"{appOptions.FrontendUrl}/reset-password?token={token}&email={email}";
            await emailService.SendPasswordResetEmail(user.Email, user.Name, resetLink);
    
            logger.LogInformation("Password reset email sent to {Email}", email);
        }

        public async Task ResetPassword(ResetPasswordRequestDto request)
        {
            var user = await userRepository.Query()
                .FirstOrDefaultAsync(u => u.Email == request.Email);
    
            if (user == null || 
                user.PasswordResetToken != request.Token || 
                user.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                throw new AuthenticationError();
            }
    
            // Update password
            user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
    
            await userRepository.UpdateAsync(user);
    
            logger.LogInformation("Password reset successful for {Email}", user.Email);
        }
    }