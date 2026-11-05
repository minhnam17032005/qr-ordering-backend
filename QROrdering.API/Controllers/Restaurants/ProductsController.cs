using Microsoft.AspNetCore.Mvc;
using QROrdering.API.Extensions;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.RestaurantManagement.Products.DTOs.Requests;
using QROrdering.Application.RestaurantManagement.Products.DTOs.Responses;
using QROrdering.Application.RestaurantManagement.Products.Interfaces;

namespace QROrdering.API.Controllers.Restaurants
{
    [ApiController]
    [Route("api/restaurants/{restaurantId:guid}/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<ProductListResponse>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PagedResponse<ProductListResponse>>>>
            GetProducts(
                Guid restaurantId,
                [FromQuery] PagedRequest request,
                [FromQuery] Guid? categoryId)
        {
            var response = await _productService.GetProductsAsync(
                restaurantId,
                categoryId,
                request);

            return this.ApiOk(response, "Lấy danh sách sản phẩm thành công.");
        }

        [HttpGet("{productId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDetailResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductDetailResponse>>>GetProductById(Guid restaurantId, Guid productId)
        {
            var response = await _productService.GetProductByIdAsync(
                restaurantId,
                productId);

            return this.ApiOk(response, "Lấy thông tin sản phẩm thành công.");
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductDetailResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductDetailResponse>>>CreateProduct(
        Guid restaurantId,
        [FromBody] ProductCreateRequest request)
        {
            var response = await _productService.CreateProductAsync(
                restaurantId,
                request);

            return this.ApiOk(response, "Tạo sản phẩm thành công.");
        }

        [HttpPut("{productId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDetailResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductDetailResponse>>>
            UpdateProduct(
                Guid restaurantId,
                Guid productId,
                [FromBody] ProductUpdateRequest request)
        {
            var response = await _productService.UpdateProductAsync(
                restaurantId,
                productId,
                request);

            return this.ApiOk(response, "Cập nhật sản phẩm thành công.");
        }

        [HttpPatch("{productId:guid}/activate")]
        [ProducesResponseType(typeof(ApiResponse<ProductStatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductStatusResponse>>>ActivateProduct(
        Guid restaurantId,
        Guid productId)
        {
            var response = await _productService.ActivateProductAsync(
                restaurantId,
                productId);

            return this.ApiOk(response, "Kích hoạt sản phẩm thành công.");
        }

        [HttpPatch("{productId:guid}/deactivate")]
        [ProducesResponseType(typeof(ApiResponse<ProductStatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductStatusResponse>>>DeactivateProduct(
                Guid restaurantId,
                Guid productId)
        {
            var response = await _productService.DeactivateProductAsync(
                restaurantId,
                productId);

            return this.ApiOk(response, "Ngừng kinh doanh sản phẩm thành công.");
        }
    }
}