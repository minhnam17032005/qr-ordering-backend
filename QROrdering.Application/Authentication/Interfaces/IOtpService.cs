using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QROrdering.Application.Authentication.DTOs.Requests;
using QROrdering.Application.Authentication.DTOs.Responses;
using QROrdering.Domain.Entities.Identity;

namespace QROrdering.Application.Authentication.Interfaces
{
    public interface IOtpService
    {
        Task SendChangePasswordOtpAsync(User user);

        Task<VerifyOtpResponse> VerifyChangePasswordOtpAsync(
            User user,
            string otp);

        Task ValidateChangePasswordVerificationAsync(
            User user,
            string verificationToken);

        Task SendForgotPasswordOtpAsync(User user);

        Task<VerifyOtpResponse> VerifyForgotPasswordOtpAsync(
            User user,
            string otp);

        Task<User> ValidateForgotPasswordVerificationAsync(
            string verificationToken);

    }
}
