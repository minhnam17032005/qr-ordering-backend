using Microsoft.AspNetCore.Mvc;
using QROrdering.API.Extensions;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.RestaurantManagement.Categories.DTOs.Requests;
using QROrdering.Application.RestaurantManagement.Categories.DTOs.Responses;
using QROrdering.Application.RestaurantManagement.Categories.Interfaces;

namespace QROrdering.API.Controllers.Restaurants
{
    [ApiController]
    [Route("api/restaurants/{restaurantId:guid}/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // ============================================================
        // GET CATEGORIES
        // ============================================================

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<CategoryListResponse>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PagedResponse<CategoryListResponse>>>> GetCategories(
            Guid restaurantId,
            [FromQuery] PagedRequest request)
        {
            var response = await _categoryService.GetCategoriesAsync(
                restaurantId,
                request);

            return this.ApiOk(
                response,
                "Lấy danh sách danh mục thành công.");
        }

        // ============================================================
        // GET CATEGORY BY ID
        // ============================================================

        [HttpGet("{categoryId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDetailResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CategoryDetailResponse>>> GetCategoryById(
            Guid restaurantId,
            Guid categoryId)
        {
            var response = await _categoryService.GetCategoryByIdAsync(
                restaurantId,
                categoryId);

            return this.ApiOk(
                response,
                "Lấy thông tin danh mục thành công.");
        }

        // ============================================================
        // CREATE CATEGORY
        // ============================================================

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CategoryDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CategoryDetailResponse>>> CreateCategory(
            Guid restaurantId,
            [FromBody] CategoryCreateRequest request)
        {
            var response = await _categoryService.CreateCategoryAsync(
                restaurantId,
                request);

            return this.ApiOk(
                response,
                "Tạo danh mục thành công.");
        }

        // ============================================================
        // UPDATE CATEGORY
        // ============================================================

        [HttpPut("{categoryId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CategoryDetailResponse>>> UpdateCategory(
            Guid restaurantId,
            Guid categoryId,
            [FromBody] CategoryUpdateRequest request)
        {
            var response = await _categoryService.UpdateCategoryAsync(
                restaurantId,
                categoryId,
                request);

            return this.ApiOk(
                response,
                "Cập nhật danh mục thành công.");
        }

        // ============================================================
        // ACTIVATE CATEGORY
        // ============================================================

        [HttpPatch("{categoryId:guid}/activate")]
        [ProducesResponseType(typeof(ApiResponse<CategoryStatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CategoryStatusResponse>>> ActivateCategory(
            Guid restaurantId,
            Guid categoryId)
        {
            var response = await _categoryService.ActivateCategoryAsync(
                restaurantId,
                categoryId);

            return this.ApiOk(
                response,
                "Kích hoạt danh mục thành công.");
        }

        // ============================================================
        // DEACTIVATE CATEGORY
        // ============================================================

        [HttpPatch("{categoryId:guid}/deactivate")]
        [ProducesResponseType(typeof(ApiResponse<CategoryStatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CategoryStatusResponse>>> DeactivateCategory(
            Guid restaurantId,
            Guid categoryId)
        {
            var response = await _categoryService.DeactivateCategoryAsync(
                restaurantId,
                categoryId);

            return this.ApiOk(
                response,
                "Ngừng hoạt động danh mục thành công.");
        }
    }
}