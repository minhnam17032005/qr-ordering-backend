using Microsoft.Extensions.Options;
using QROrdering.Application.Common.Configurations;
using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Exceptions;
using QROrdering.Domain.Enums;
using QROrdering.Infrastructure.Redis;
using QROrdering.Infrastructure.Redis.Models;
using QROrdering.Infrastructure.Helpers;
using QROrdering.Application.Common.DTOs;
using QROrdering.Application.Common.Enums;

namespace QROrdering.Application.Authentication
{
    public class OtpService : IOtpService
    {
        private readonly IEmailService _emailService;
        private readonly IRedisService _redisService;
        private readonly IHashService _hashService;
        private readonly OtpSettings _otpSettings;

        public OtpService(
            IEmailService emailService,
            IRedisService redisService,
            IHashService hashService,
            IOptions<OtpSettings> otpSettings)
        {
            _emailService = emailService;
            _redisService = redisService;
            _hashService = hashService;
            _otpSettings = otpSettings.Value;
        }

        // Gửi OTP cho chức năng đổi mật khẩu.
        public async Task SendChangePasswordOtpAsync(OtpAccount account)
        {
            await SendOtpAsync(
                account,
                OtpType.ChangePassword,
                "Change Password OTP",
                "Bạn vừa yêu cầu thay đổi mật khẩu. Vui lòng sử dụng mã OTP bên dưới để tiếp tục.");
        }

        // Xác thực OTP đổi mật khẩu và trả về Verification Token.
        public async Task<OtpVerificationResult> VerifyChangePasswordOtpAsync(
        OtpAccount account,
        string otp)
        {
            return await VerifyOtpAsync(
                account,
                otp,
                OtpType.ChangePassword);
        }

        // Xác thực Verification Token đổi mật khẩu.
        public async Task ValidateChangePasswordVerificationAsync(
        OtpAccount account,
        string verificationToken)
        {
            var verificationKey =
                GetChangePasswordVerificationKey(account);

            var verification =
                await _redisService.GetAsync<RedisChangePasswordVerification>(
                    verificationKey);

            if (verification == null)
            {
                throw new BadRequestException(
                    "Verification token không hợp lệ hoặc đã hết hạn.");
            }

            var isTokenValid = _hashService.Verify(
                verificationToken,
                verification.TokenHash);

            if (!isTokenValid)
            {
                throw new BadRequestException(
                    "Verification token không hợp lệ.");
            }

            // Verification token chỉ được sử dụng một lần.
            await _redisService.RemoveAsync(verificationKey);
        }

        // Gửi OTP cho chức năng quên mật khẩu.
        public async Task SendForgotPasswordOtpAsync(OtpAccount account)
        {
            await SendOtpAsync(
                account,
                OtpType.ForgotPassword,
                "Forgot Password OTP",
                "Bạn vừa yêu cầu đặt lại mật khẩu. Vui lòng sử dụng mã OTP bên dưới để tiếp tục.");
        }

        // Xác thực OTP quên mật khẩu và trả về Verification Token.
        public async Task<OtpVerificationResult> VerifyForgotPasswordOtpAsync(
        OtpAccount account,
        string otp)
        {
            return await VerifyOtpAsync(
                account,
                otp,
                OtpType.ForgotPassword);
        }

        // Xác thực Verification Token quên mật khẩu.
        public async Task<string> ValidateForgotPasswordVerificationAsync(
        string verificationToken,
        OtpAccountType accountType)
        {
            var verificationKey =
                GetForgotPasswordVerificationKey(
                    verificationToken,
                    accountType);

            var verification =
                await _redisService.GetAsync<RedisForgotPasswordVerification>(
                    verificationKey);

            if (verification == null)
            {
                throw new BadRequestException(
                    "Verification token không hợp lệ hoặc đã hết hạn.");
            }

            await _redisService.RemoveAsync(verificationKey);

            return verification.Email;
        }

        // Tạo OTP, lưu Redis và gửi Email.
        // Tạo OTP, lưu Redis và gửi Email.
        private async Task SendOtpAsync(
            OtpAccount account,
            OtpType otpType,
            string subject,
            string message)
        {
            var otpKey =
                GetOtpKey(account, otpType);

            var cooldownKey =
                GetCooldownKey(account, otpType);

            var rateLimitKey =
                GetRateLimitKey(account, otpType);

            // Check cooldown.
            if (await _redisService.ExistsAsync(cooldownKey))
            {
                throw new BadRequestException(
                    "Vui lòng đợi trước khi gửi OTP tiếp.");
            }

            // Check rate limit.
            var rateLimit =
                await _redisService.GetAsync<RedisOtpRateLimit>(
                    rateLimitKey);

            if (rateLimit == null)
            {
                rateLimit = new RedisOtpRateLimit
                {
                    Count = 0
                };
            }

            if (rateLimit.Count >= _otpSettings.MaxSendPerWindow)
            {
                throw new BadRequestException(
                    "Bạn đã gửi OTP quá nhiều lần. Vui lòng thử lại sau.");
            }

            // Generate OTP.
            var otp = OtpGenerator.GenerateOtp();

            var redisOtp = new RedisEmailOtp
            {
                OtpHash = _hashService.Hash(otp),
                FailedAttempts = 0
            };

            // Save OTP.
            await _redisService.SetAsync(
                otpKey,
                redisOtp,
                TimeSpan.FromMinutes(
                    _otpSettings.ExpiredMinutes));

            // Send Email.
            await _emailService.SendOtpAsync(
                account.Email,
                subject,
                message,
                otp);

            // Start cooldown.
            await _redisService.SetAsync(
                cooldownKey,
                new RedisOtpCooldown(),
                TimeSpan.FromSeconds(
                    _otpSettings.ResendCooldownSeconds));

            // Increase rate limit.
            rateLimit.Count++;

            var ttl =
                await _redisService.GetTimeToLiveAsync(
                    rateLimitKey);

            await _redisService.SetAsync(
                rateLimitKey,
                rateLimit,
                ttl ?? TimeSpan.FromMinutes(
                    _otpSettings.RateLimitWindowMinutes));
        }

        private async Task<OtpVerificationResult> VerifyOtpAsync(
        OtpAccount account,
        string otp,
        OtpType otpType)
        {
            var expiry = TimeSpan.FromMinutes(
                _otpSettings.VerificationExpiredMinutes);

            var otpKey =
                GetOtpKey(account, otpType);

            // Get OTP from Redis.
            var redisOtp =
                await _redisService.GetAsync<RedisEmailOtp>(
                    otpKey);

            if (redisOtp == null)
            {
                throw new BadRequestException(
                    "OTP không tồn tại hoặc đã hết hạn.");
            }

            // Verify OTP.
            if (!_hashService.Verify(
                    otp,
                    redisOtp.OtpHash))
            {
                redisOtp.FailedAttempts++;

                if (redisOtp.FailedAttempts >=
                    _otpSettings.MaxVerifyAttempts)
                {
                    await _redisService.RemoveAsync(otpKey);

                    throw new BadRequestException(
                        "OTP đã bị khóa do nhập sai quá nhiều lần. Vui lòng yêu cầu OTP mới.");
                }

                var ttl =
                    await _redisService.GetTimeToLiveAsync(
                        otpKey);

                await _redisService.SetAsync(
                    otpKey,
                    redisOtp,
                    ttl);

                var remaining =
                    _otpSettings.MaxVerifyAttempts -
                    redisOtp.FailedAttempts;

                throw new BadRequestException(
                    $"OTP không chính xác. Bạn còn {remaining} lần thử.");
            }

            // OTP đúng -> xóa OTP.
            await _redisService.RemoveAsync(otpKey);

            // Generate Verification Token.
            var verificationToken =
                TokenGenerator.GenerateSecureToken();

            if (otpType == OtpType.ChangePassword)
            {
                await _redisService.SetAsync(
                    GetChangePasswordVerificationKey(account),
                    new RedisChangePasswordVerification
                    {
                        TokenHash =
                            _hashService.Hash(verificationToken)
                    },
                    expiry);
            }
            else
            {
                await _redisService.SetAsync(
                    GetForgotPasswordVerificationKey(
                        verificationToken,
                        account.AccountType),
                    new RedisForgotPasswordVerification
                    {
                        Email = account.Email
                    },
                    expiry);
            }

            return new OtpVerificationResult
            {
                VerificationToken = verificationToken,
                ExpiredAt = DateTime.UtcNow.Add(expiry)
            };
        }

        private static string GetOtpKey(
        OtpAccount account,
        OtpType otpType)
        {
            if (account.AccountType == OtpAccountType.User)
            {
                return otpType == OtpType.ChangePassword
                    ? RedisKeys.ChangePasswordOtp(account.Id)
                    : RedisKeys.ForgotPasswordOtp(account.Email);
            }

            return otpType == OtpType.ChangePassword
                ? RedisKeys.PlatformAdminChangePasswordOtp(account.Id)
                : RedisKeys.PlatformAdminForgotPasswordOtp(account.Email);
        }

        private static string GetCooldownKey(
        OtpAccount account,
        OtpType otpType)
        {
            if (account.AccountType == OtpAccountType.User)
            {
                return otpType == OtpType.ChangePassword
                    ? RedisKeys.ChangePasswordCooldown(account.Id)
                    : RedisKeys.ForgotPasswordCooldown(account.Email);
            }

            return otpType == OtpType.ChangePassword
                ? RedisKeys.PlatformAdminChangePasswordCooldown(account.Id)
                : RedisKeys.PlatformAdminForgotPasswordCooldown(account.Email);
        }

        private static string GetRateLimitKey(
        OtpAccount account,
        OtpType otpType)
        {
            if (account.AccountType == OtpAccountType.User)
            {
                return otpType == OtpType.ChangePassword
                    ? RedisKeys.ChangePasswordRateLimit(account.Id)
                    : RedisKeys.ForgotPasswordRateLimit(account.Email);
            }

            return otpType == OtpType.ChangePassword
                ? RedisKeys.PlatformAdminChangePasswordRateLimit(account.Id)
                : RedisKeys.PlatformAdminForgotPasswordRateLimit(account.Email);
        }

        private static string GetChangePasswordVerificationKey(
        OtpAccount account)
        {
            return account.AccountType == OtpAccountType.User
                ? RedisKeys.ChangePasswordVerification(account.Id)
                : RedisKeys.PlatformAdminChangePasswordVerification(
                    account.Id);
        }

        private static string GetForgotPasswordVerificationKey(
        string verificationToken,
        OtpAccountType accountType)
        {
            return accountType == OtpAccountType.User
                ? RedisKeys.ForgotPasswordVerification(
                    verificationToken)
                : RedisKeys.PlatformAdminForgotPasswordVerification(
                    verificationToken);
        }

    }
}