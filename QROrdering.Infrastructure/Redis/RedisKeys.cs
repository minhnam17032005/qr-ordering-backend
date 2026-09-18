using System;

namespace QROrdering.Infrastructure.Redis
{
    // Quản lý tên Redis Key
    public static class RedisKeys
    {
        // =====================================================
        // USER
        // =====================================================

        public static string Permission(Guid userId)
            => $"qroordering:permissions:{userId}";

        public static string Session(Guid sessionId)
            => $"qroordering:session:{sessionId}";

        // Change Password
        // Đã đăng nhập -> biết UserId

        public static string ChangePasswordOtp(Guid userId)
            => $"qroordering:otp:change:{userId}";

        public static string ChangePasswordVerification(Guid userId)
            => $"qroordering:verify:change:{userId}";

        public static string ChangePasswordCooldown(Guid userId)
            => $"qroordering:cooldown:change:{userId}";

        public static string ChangePasswordRateLimit(Guid userId)
            => $"qroordering:ratelimit:change:{userId}";

        // Forgot Password
        // Chưa đăng nhập -> dùng Email

        public static string ForgotPasswordOtp(string email)
            => $"qroordering:otp:forgot:{NormalizeEmail(email)}";

        public static string ForgotPasswordVerification(
            string verificationToken)
            => $"qroordering:verify:forgot:{verificationToken}";

        public static string ForgotPasswordCooldown(string email)
            => $"qroordering:cooldown:forgot:{NormalizeEmail(email)}";

        public static string ForgotPasswordRateLimit(string email)
            => $"qroordering:ratelimit:forgot:{NormalizeEmail(email)}";


        // =====================================================
        // PLATFORM ADMIN
        // =====================================================

        public static string PlatformAdminSession(Guid sessionId)
            => $"qroordering:platform:session:{sessionId}";

        // Change Password
        // Đã đăng nhập -> biết PlatformAdminId

        public static string PlatformAdminChangePasswordOtp(
            Guid platformAdminId)
            => $"qroordering:platform:otp:change:{platformAdminId}";

        public static string PlatformAdminChangePasswordVerification(
            Guid platformAdminId)
            => $"qroordering:platform:verify:change:{platformAdminId}";

        public static string PlatformAdminChangePasswordCooldown(
            Guid platformAdminId)
            => $"qroordering:platform:cooldown:change:{platformAdminId}";

        public static string PlatformAdminChangePasswordRateLimit(
            Guid platformAdminId)
            => $"qroordering:platform:ratelimit:change:{platformAdminId}";

        // Forgot Password
        // Chưa đăng nhập -> dùng Email

        public static string PlatformAdminForgotPasswordOtp(
            string email)
            => $"qroordering:platform:otp:forgot:{NormalizeEmail(email)}";

        public static string PlatformAdminForgotPasswordVerification(
            string verificationToken)
            => $"qroordering:platform:verify:forgot:{verificationToken}";

        public static string PlatformAdminForgotPasswordCooldown(
            string email)
            => $"qroordering:platform:cooldown:forgot:{NormalizeEmail(email)}";

        public static string PlatformAdminForgotPasswordRateLimit(
            string email)
            => $"qroordering:platform:ratelimit:forgot:{NormalizeEmail(email)}";


        // =====================================================
        // JWT BLACKLIST
        // User + Platform Admin dùng chung
        // =====================================================

        public static string BlacklistToken(string jti)
            => $"qroordering:blacklist:jwt:{jti}";


        // =====================================================
        // HELPER
        // =====================================================

        private static string NormalizeEmail(string email)
            => email.Trim().ToLowerInvariant();
    }
}