using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using QROrdering.Application.Common.Responses;

namespace QROrdering.Infrastructure.Authentication
{
    public class JwtAuthEvents : JwtBearerEvents
    {
        private readonly ILogger<JwtAuthEvents> _logger;

        public JwtAuthEvents(
            ILogger<JwtAuthEvents> logger)
        {
            _logger = logger;
        }

        // TOKEN INVALID / EXPIRED
        public override async Task AuthenticationFailed(
            AuthenticationFailedContext context)
        {
            context.NoResult();

            _logger.LogWarning(
                context.Exception,
                "JWT Authentication failed. Method: {Method}, Path: {Path}",
                context.HttpContext.Request.Method,
                context.HttpContext.Request.Path);

            // Đánh dấu AuthenticationFailed đã xử lý
            context.HttpContext.Items["AuthFailed"] = true;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;

            string code = "TOKEN_INVALID";
            string message = "Access token không hợp lệ";

            // TOKEN EXPIRED
            if (context.Exception is SecurityTokenExpiredException)
            {
                code = "TOKEN_EXPIRED";
                message = "Access token đã hết hạn";
            }

            // INVALID SIGNATURE
            else if (
                context.Exception
                is SecurityTokenInvalidSignatureException)
            {
                code = "INVALID_SIGNATURE";
                message = "Chữ ký token không hợp lệ";
            }

            // MALFORMED TOKEN
            else if (
                context.Exception
                    is SecurityTokenMalformedException
                || context.Exception is ArgumentException)
            {
                code = "MALFORMED_TOKEN";
                message = "Token không đúng định dạng";
            }

            var response = new ErrorResponse
            {
                StatusCode =
                    StatusCodes.Status401Unauthorized,

                Code = code,

                Message = message
            };

            await context.Response.WriteAsJsonAsync(response);
        }

        // NO TOKEN
        public override async Task Challenge(
            JwtBearerChallengeContext context)
        {
            // AuthenticationFailed đã xử lý
            if (context.HttpContext.Items.ContainsKey("AuthFailed"))
            {
                return;
            }

            // Response đã được ghi
            if (context.Response.HasStarted)
            {
                return;
            }

            context.HandleResponse();

            _logger.LogWarning(
                "JWT Authentication challenge. Missing access token. " +
                "Method: {Method}, Path: {Path}",
                context.HttpContext.Request.Method,
                context.HttpContext.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;

            var response = new ErrorResponse
            {
                StatusCode =
                    StatusCodes.Status401Unauthorized,

                Code = "TOKEN_MISSING",

                Message = "Vui lòng đăng nhập"
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
