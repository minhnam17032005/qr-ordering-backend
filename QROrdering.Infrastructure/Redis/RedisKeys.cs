using System;

namespace QROrdering.Infrastructure.Redis
{
    // Quản lý tên Redis Key
    public static class RedisKeys
    {

        public static string Permission(Guid userId)
            => $"qroordering:permissions:{userId}";

        public static string Session(Guid sessionId)
            => $"qroordering:session:{sessionId}";


        public static string BlacklistToken(string jti)
            => $"qroordering:blacklist:jwt:{jti}";


        // =====================================================
        // CHANGE PASSWORD
        // Đã đăng nhập -> biết UserId
        // =====================================================

        public static string ChangePasswordOtp(Guid userId)
            => $"qroordering:otp:change:{userId}";

        public static string ChangePasswordVerification(Guid userId)
            => $"qroordering:verify:change:{userId}";

        public static string ChangePasswordCooldown(Guid userId)
            => $"qroordering:cooldown:change:{userId}";

        public static string ChangePasswordRateLimit(Guid userId)
            => $"qroordering:ratelimit:change:{userId}";


        // =====================================================
        // FORGOT PASSWORD
        // Chưa đăng nhập -> dùng Email
        // =====================================================

        public static string ForgotPasswordOtp(string email)
            => $"qroordering:otp:forgot:{NormalizeEmail(email)}";

        public static string ForgotPasswordVerification(
            string verificationToken)
            => $"qroordering:verify:forgot:{verificationToken}";

        public static string ForgotPasswordCooldown(string email)
            => $"qroordering:cooldown:forgot:{NormalizeEmail(email)}";

        public static string ForgotPasswordRateLimit(string email)
            => $"qroordering:ratelimit:forgot:{NormalizeEmail(email)}";


        // =========================
        // Helper
        // =========================
        private static string NormalizeEmail(string email)
            => email.Trim().ToLowerInvariant();
    }
}