using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QROrdering.API.Extensions;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.Platform.Registrations.DTOs.Responses;
using QROrdering.Application.Platform.Registrations.Interfaces;

namespace QROrdering.API.Controllers.Platform
{
    [ApiController]
    [Route("api/platform/registrations")]
    [Authorize]
    public class PlatformRegistrationsController: ControllerBase
    {
        private readonly IPlatformServiceRegistrationService _registrationService;

        public PlatformRegistrationsController(
            IPlatformServiceRegistrationService
                registrationService)
        {
            _registrationService =
                registrationService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<PlatformServiceRegistrationListResponse>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PagedResponse<PlatformServiceRegistrationListResponse>>>>GetRegistrations(
                [FromQuery] PagedRequest request)
        {
            var response =
                await _registrationService
                    .GetRegistrationsAsync(request);

            return this.ApiOk(
                response,
                "Lấy danh sách đăng ký thành công.");
        }

        [HttpGet("{registrationId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PlatformServiceRegistrationDetailResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult< ApiResponse< PlatformServiceRegistrationDetailResponse>>>GetRegistrationById(
                Guid registrationId)
        {
            var response =
                await _registrationService
                    .GetRegistrationByIdAsync(
                        registrationId);

            return this.ApiOk(
                response,
                "Lấy thông tin đơn đăng ký thành công.");
        }

        [HttpPost("{registrationId:guid}/approve")]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>>ApproveRegistration(
            Guid registrationId)
        {
            await _registrationService
                .ApproveRegistrationAsync(
                    registrationId);

            return this.ApiOk<object>(
                null,
                "Duyệt đơn đăng ký thành công.");
        }

        [HttpPost("{registrationId:guid}/reject")]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>>RejectRegistration(
        Guid registrationId)
        {
            await _registrationService
                .RejectRegistrationAsync(
                    registrationId);

            return this.ApiOk<object>(
                null,
                "Từ chối đơn đăng ký thành công.");
        }
    }
}