using Microsoft.AspNetCore.Mvc;
using QROrdering.API.Extensions;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.RestaurantManagement.Tables.DTOs.Requests;
using QROrdering.Application.RestaurantManagement.Tables.DTOs.Responses;
using QROrdering.Application.RestaurantManagement.Tables.Interfaces;

namespace QROrdering.API.Controllers.RestaurantManagement
{
    [ApiController]
    [Route("api/restaurants/{restaurantId:guid}/tables")]
    public class RestaurantTablesController : ControllerBase
    {
        private readonly IRestaurantTableService _tableService;

        public RestaurantTablesController(
            IRestaurantTableService tableService)
        {
            _tableService = tableService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<TableListResponse>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PagedResponse<TableListResponse>>>>GetTables(
                Guid restaurantId,
                [FromQuery] PagedRequest request)
        {
            var response = await _tableService.GetTablesAsync(
                restaurantId,
                request);

            return this.ApiOk(response, "Lấy danh sách bàn thành công.");
        }

        [HttpGet("{tableId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<TableDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TableDetailResponse>>>GetTableById(
                Guid restaurantId,
                Guid tableId)
        {
            var response = await _tableService.GetTableByIdAsync(
                restaurantId,
                tableId);

            return this.ApiOk(response, "Lấy thông tin bàn thành công.");
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TableDetailResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TableDetailResponse>>>CreateTable(
                Guid restaurantId,
                [FromBody] TableCreateRequest request)
        {
            var response = await _tableService.CreateTableAsync(
                restaurantId,
                request);

            return this.ApiOk(response, "Tạo bàn thành công.");
        }

        [HttpPut("{tableId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<TableDetailResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType( typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TableDetailResponse>>>UpdateTable(
            Guid restaurantId,Guid tableId,
                [FromBody] TableUpdateRequest request)
        {
            var response = await _tableService.UpdateTableAsync(
                restaurantId,
                tableId,
                request);

            return this.ApiOk(response, "Cập nhật bàn thành công.");
        }

        [HttpPatch("{tableId:guid}/activate")]
        [ProducesResponseType(typeof(ApiResponse<TableStatusResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TableStatusResponse>>>ActivateTable(
                Guid restaurantId,
                Guid tableId)
        {
            var response = await _tableService.ActivateTableAsync(
                restaurantId,
                tableId);

            return this.ApiOk(response, "Kích hoạt bàn thành công.");
        }

        [HttpPatch("{tableId:guid}/deactivate")]
        [ProducesResponseType(typeof(ApiResponse<TableStatusResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TableStatusResponse>>>DeactivateTable(
                Guid restaurantId,
                Guid tableId)
        {
            var response = await _tableService.DeactivateTableAsync(
                restaurantId,
                tableId);

            return this.ApiOk(response, "Vô hiệu hóa bàn thành công.");
        }
    }
}