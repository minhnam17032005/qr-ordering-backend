using Microsoft.Extensions.Options;
using QROrdering.Application.Common.Configurations;
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Exceptions;
using QROrdering.Domain.Entities.Identity;
using QROrdering.Domain.Enums;
using QROrdering.Infrastructure.Redis;
using QROrdering.Infrastructure.Redis.Models;
using QROrdering.Infrastructure.Helpers;
using QROrdering.Application.Authentication.DTOs.Responses;
using QROrdering.Application.Authentication.DTOs.Requests;
using QROrdering.Infrastructure.Authentication;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Application.Authentication
{
    public class OtpService : IOtpService
    {
        private readonly IEmailService _emailService;
        private readonly IRedisService _redisService;
        private readonly IHashService _hashService;
        private readonly IUserRepository _userRepository;
        private readonly OtpSettings _otpSettings;

        public OtpService(
            IEmailService emailService,
            IRedisService redisService,
            IHashService hashService,
            IUserRepository userRepository,
            IOptions<OtpSettings> otpSettings)
        {
            _emailService = emailService;
            _redisService = redisService;
            _hashService = hashService;
            _userRepository = userRepository;
            _otpSettings = otpSettings.Value;
        }

        // Gửi OTP cho chức năng đổi mật khẩu.
        public async Task SendChangePasswordOtpAsync(User user)
        {
            await SendOtpAsync(
                user,
                OtpType.ChangePassword,
                "Change Password OTP",
                "Bạn vừa yêu cầu thay đổi mật khẩu. Vui lòng sử dụng mã OTP bên dưới để tiếp tục.");
        }

        // Xác thực OTP đổi mật khẩu và trả về Verification Token.
        public async Task<VerifyOtpResponse> VerifyChangePasswordOtpAsync(
            User user,
            string otp)
        {
            return await VerifyOtpAsync(
                user,
                otp,
                OtpType.ChangePassword);
        }

        // Xác thực Verification Token đổi mật khẩu.
        public async Task ValidateChangePasswordVerificationAsync(
            User user,
            string verificationToken)
        {
            var verificationKey =
                RedisKeys.ChangePasswordVerification(user.Id);

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
        public async Task SendForgotPasswordOtpAsync(User user)
        {
            await SendOtpAsync(
                user,
                OtpType.ForgotPassword,
                "Forgot Password OTP",
                "Bạn vừa yêu cầu đặt lại mật khẩu. Vui lòng sử dụng mã OTP bên dưới để tiếp tục.");
        }

        // Xác thực OTP quên mật khẩu và trả về Verification Token.
        public async Task<VerifyOtpResponse> VerifyForgotPasswordOtpAsync(
            User user,
            string otp)
        {
            return await VerifyOtpAsync(
                user,
                otp,
                OtpType.ForgotPassword);
        }

        // Xác thực Verification Token quên mật khẩu.
        public async Task<User> ValidateForgotPasswordVerificationAsync(
            string verificationToken)
        {
            var verificationKey =
                RedisKeys.ForgotPasswordVerification(
                    verificationToken);

            var verification =
                await _redisService.GetAsync<RedisForgotPasswordVerification>(
                    verificationKey);

            if (verification == null)
            {
                throw new BadRequestException(
                    "Verification token không hợp lệ hoặc đã hết hạn.");
            }

            var user = await _userRepository
                .GetByEmailAsync(verification.Email);

            if (user == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy người dùng.");
            }

            // Verification token chỉ được sử dụng một lần.
            await _redisService.RemoveAsync(verificationKey);

            return user;
        }

        // Tạo OTP, lưu Redis và gửi Email.
        private async Task SendOtpAsync(
            User user,
            OtpType otpType,
            string subject,
            string message)
        {
            var otpKey = otpType == OtpType.ChangePassword
                ? RedisKeys.ChangePasswordOtp(user.Id)
                : RedisKeys.ForgotPasswordOtp(user.Email);

            var cooldownKey = otpType == OtpType.ChangePassword
                ? RedisKeys.ChangePasswordCooldown(user.Id)
                : RedisKeys.ForgotPasswordCooldown(user.Email);

            var rateLimitKey = otpType == OtpType.ChangePassword
                ? RedisKeys.ChangePasswordRateLimit(user.Id)
                : RedisKeys.ForgotPasswordRateLimit(user.Email);

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
                user.Email,
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

        private async Task<VerifyOtpResponse> VerifyOtpAsync(
            User user,
            string otp,
            OtpType otpType)
        {
            var expiry = TimeSpan.FromMinutes(
                _otpSettings.VerificationExpiredMinutes);

            var otpKey = otpType == OtpType.ChangePassword
                ? RedisKeys.ChangePasswordOtp(user.Id)
                : RedisKeys.ForgotPasswordOtp(user.Email);

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
                    RedisKeys.ChangePasswordVerification(user.Id),
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
                    RedisKeys.ForgotPasswordVerification(
                        verificationToken),
                    new RedisForgotPasswordVerification
                    {
                        Email = user.Email
                    },
                    expiry);
            }

            return new VerifyOtpResponse
            {
                VerificationToken = verificationToken,
                ExpiredAt = DateTime.UtcNow.Add(expiry)
            };
        }

        
    }
}