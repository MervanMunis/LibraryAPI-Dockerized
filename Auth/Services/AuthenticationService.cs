using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.Entities;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LibraryAPI.Auth.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }
        public async Task<ServiceResult<string>> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordRequest.Email);
            if (user == null)
            {
                return ServiceResult<string>.FailureResult("User not found.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // Here we need to send the token via email to the user
            // For simplicity, we're returning the token as a response (do not do this in a real application)
            return ServiceResult<string>.SuccessResult(token);
        }

        public async Task<ServiceResult<string>> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);
            if (user == null)
            {
                return ServiceResult<string>.FailureResult("User not found.");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordRequest.Token, resetPasswordRequest.NewPassword);
            if (!result.Succeeded)
            {
                return ServiceResult<string>.FailureResult("Failed to reset password.");
            }

            return ServiceResult<string>.SuccessResult("Password reset successfully.");
        }
    }
}
