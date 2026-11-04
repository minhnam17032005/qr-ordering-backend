
using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Exceptions;
using QROrdering.Application.RestaurantManagement.Categories.DTOs.Requests;
using QROrdering.Application.RestaurantManagement.Categories.DTOs.Responses;
using QROrdering.Application.RestaurantManagement.Categories.Interfaces;
using QROrdering.Domain.Entities.RestaurantManagement;

namespace QROrdering.Application.RestaurantManagement.Categories
{
    public class CategoryService : ICategoryService
    {
        private const int MaxPageSize = 100;

        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        // ============================================================
        // GET ALL CATEGORIES
        // ============================================================

        public async Task<PagedResponse<CategoryListResponse>> GetCategoriesAsync(
            Guid restaurantId,
            PagedRequest request)
        {
            if (restaurantId == Guid.Empty)
                throw new BadRequestException("RestaurantId không hợp lệ.");

            if (request is null)
                throw new BadRequestException("Dữ liệu phân trang không hợp lệ.");

            if (request.Page < 1 ||
                request.PageSize < 1 ||
                request.PageSize > MaxPageSize)
            {
                throw new BadRequestException(
                    $"Page phải lớn hơn hoặc bằng 1 và PageSize phải từ 1 đến {MaxPageSize}.");
            }

            var restaurantExists =
                await _categoryRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
                throw new NotFoundException("Không tìm thấy nhà hàng.");

            request.Search = NormalizeOptionalText(request.Search);

            var totalItems =
                await _categoryRepository.CountByRestaurantAsync(
                    restaurantId,
                    request.Search);

            var items =
                await _categoryRepository.GetPagedByRestaurantAsync(
                    restaurantId,
                    request);

            var totalPages = (int)Math.Ceiling(
                (double)totalItems / request.PageSize);

            return new PagedResponse<CategoryListResponse>
            {
                Items = items,
                Pagination = new PaginationMeta
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages
                }
            };
        }

        // ============================================================
        // GET CATEGORY BY ID
        // ============================================================

        public async Task<CategoryDetailResponse> GetCategoryByIdAsync(
            Guid restaurantId,
            Guid categoryId)
        {
            ValidateIds(restaurantId, categoryId);

            var restaurantExists =
                await _categoryRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
                throw new NotFoundException("Không tìm thấy nhà hàng.");

            var category =
                await _categoryRepository.GetByIdInRestaurantAsync(
                    restaurantId,
                    categoryId);

            if (category is null)
                throw new NotFoundException(
                    "Không tìm thấy danh mục trong nhà hàng này.");

            return category;
        }

        // ============================================================
        // CREATE CATEGORY
        // ============================================================

        public async Task<CategoryDetailResponse> CreateCategoryAsync(
            Guid restaurantId,
            CategoryCreateRequest request)
        {
            if (restaurantId == Guid.Empty)
                throw new BadRequestException("RestaurantId không hợp lệ.");

            if (request is null)
                throw new BadRequestException("Dữ liệu danh mục không hợp lệ.");

            var restaurantExists =
                await _categoryRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
                throw new NotFoundException("Không tìm thấy nhà hàng.");

            var name = NormalizeRequiredText(request.Name);
            var description = NormalizeOptionalText(request.Description);
            var imageUrl = NormalizeOptionalText(request.ImageUrl);

            ValidateCategoryData(name, description, imageUrl);

            var exists = await _categoryRepository.ExistsByNameAsync(
                restaurantId,
                name);

            if (exists)
                throw new BadRequestException(
                    "Tên danh mục đã tồn tại trong nhà hàng.");

            var category = new Category
            {
                RestaurantId = restaurantId,
                Name = name,
                Description = description,
                ImageUrl = imageUrl,
                IsActive = true
            };

            await _categoryRepository.AddAsync(category);

            await _unitOfWork.SaveChangesAsync();

            var response =
                await _categoryRepository.GetByIdInRestaurantAsync(
                    restaurantId,
                    category.Id);

            if (response is null)
                throw new NotFoundException(
                    "Không tìm thấy danh mục vừa tạo.");

            return response;
        }

        // ============================================================
        // UPDATE CATEGORY
        // ============================================================

        public async Task<CategoryDetailResponse> UpdateCategoryAsync(
            Guid restaurantId,
            Guid categoryId,
            CategoryUpdateRequest request)
        {
            ValidateIds(restaurantId, categoryId);

            if (request is null)
                throw new BadRequestException("Dữ liệu danh mục không hợp lệ.");

            var restaurantExists =
                await _categoryRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
                throw new NotFoundException("Không tìm thấy nhà hàng.");

            var category =
                await _categoryRepository.GetEntityByIdInRestaurantAsync(
                    restaurantId,
                    categoryId);

            if (category is null)
                throw new NotFoundException(
                    "Không tìm thấy danh mục trong nhà hàng này.");

            var name = NormalizeRequiredText(request.Name);
            var description = NormalizeOptionalText(request.Description);
            var imageUrl = NormalizeOptionalText(request.ImageUrl);

            ValidateCategoryData(name, description, imageUrl);

            var exists = await _categoryRepository.ExistsByNameAsync(
                restaurantId,
                name,
                categoryId);

            if (exists)
                throw new BadRequestException(
                    "Tên danh mục đã tồn tại trong nhà hàng.");

            // Only update editable fields.
            category.Name = name;
            category.Description = description;
            category.ImageUrl = imageUrl;
            category.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            var response =
                await _categoryRepository.GetByIdInRestaurantAsync(
                    restaurantId,
                    categoryId);

            if (response is null)
                throw new NotFoundException(
                    "Không tìm thấy danh mục sau khi cập nhật.");

            return response;
        }

        // ============================================================
        // ACTIVATE CATEGORY
        // ============================================================

        public async Task<CategoryStatusResponse> ActivateCategoryAsync(
            Guid restaurantId,
            Guid categoryId)
        {
            ValidateIds(restaurantId, categoryId);

            var restaurantExists =
                await _categoryRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
                throw new NotFoundException("Không tìm thấy nhà hàng.");

            var category =
                await _categoryRepository.GetEntityByIdInRestaurantAsync(
                    restaurantId,
                    categoryId);

            if (category is null)
                throw new NotFoundException(
                    "Không tìm thấy danh mục trong nhà hàng này.");

            if (category.IsActive)
                throw new BadRequestException("Danh mục đang hoạt động.");

            category.IsActive = true;
            category.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return new CategoryStatusResponse
            {
                Id = category.Id,
                IsActive = category.IsActive
            };
        }

        // ============================================================
        // DEACTIVATE CATEGORY
        // ============================================================

        public async Task<CategoryStatusResponse> DeactivateCategoryAsync(
            Guid restaurantId,
            Guid categoryId)
        {
            ValidateIds(restaurantId, categoryId);

            var restaurantExists =
                await _categoryRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
                throw new NotFoundException("Không tìm thấy nhà hàng.");

            var category =
                await _categoryRepository.GetEntityByIdInRestaurantAsync(
                    restaurantId,
                    categoryId);

            if (category is null)
                throw new NotFoundException(
                    "Không tìm thấy danh mục trong nhà hàng này.");

            if (!category.IsActive)
                throw new BadRequestException("Danh mục đã ngừng hoạt động.");

            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return new CategoryStatusResponse
            {
                Id = category.Id,
                IsActive = category.IsActive
            };
        }

        // ============================================================
        // VALIDATION
        // ============================================================

        private static void ValidateIds(
            Guid restaurantId,
            Guid categoryId)
        {
            if (restaurantId == Guid.Empty || categoryId == Guid.Empty)
                throw new BadRequestException(
                    "RestaurantId hoặc CategoryId không hợp lệ.");
        }

        private static void ValidateCategoryData(
            string name,
            string? description,
            string? imageUrl)
        {
            if (name.Length > 100)
                throw new BadRequestException(
                    "Tên danh mục không được vượt quá 100 ký tự.");

            if (description?.Length > 500)
                throw new BadRequestException(
                    "Mô tả không được vượt quá 500 ký tự.");

            if (imageUrl?.Length > 500)
                throw new BadRequestException(
                    "ImageUrl không được vượt quá 500 ký tự.");
        }

        private static string NormalizeRequiredText(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BadRequestException(
                    "Tên danh mục không được để trống.");

            return value.Trim();
        }

        private static string? NormalizeOptionalText(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }
    }
}
