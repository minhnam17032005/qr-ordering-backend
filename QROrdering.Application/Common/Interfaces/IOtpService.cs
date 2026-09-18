using QROrdering.Application.Common.DTOs;
using QROrdering.Application.Common.Enums;

namespace QROrdering.Application.Common.Interfaces
{
    public interface IOtpService
    {
        Task SendChangePasswordOtpAsync(
            OtpAccount account);

        Task<OtpVerificationResult> VerifyChangePasswordOtpAsync(
            OtpAccount account,
            string otp);

        Task ValidateChangePasswordVerificationAsync(
            OtpAccount account,
            string verificationToken);

        Task SendForgotPasswordOtpAsync(
            OtpAccount account);

        Task<OtpVerificationResult> VerifyForgotPasswordOtpAsync(
            OtpAccount account,
            string otp);

        Task<string> ValidateForgotPasswordVerificationAsync(
            string verificationToken,
            OtpAccountType accountType);
    }
}
