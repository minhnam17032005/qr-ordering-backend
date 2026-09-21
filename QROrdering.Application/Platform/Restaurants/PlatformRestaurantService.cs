using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Exceptions;
using QROrdering.Application.Platform.Restaurants.DTOs.Requests;
using QROrdering.Application.Platform.Restaurants.DTOs.Responses;
using QROrdering.Application.Platform.Restaurants.Interfaces;
using QROrdering.Domain.Entities.RestaurantManagement;

namespace QROrdering.Application.Platform.Restaurants
{
    public class PlatformRestaurantService: IPlatformRestaurantService
    {
        private readonly IPlatformRestaurantRepository _restaurantRepository;

        private readonly IUnitOfWork _unitOfWork;

        public PlatformRestaurantService(
            IPlatformRestaurantRepository restaurantRepository,
            IUnitOfWork unitOfWork)
        {
            _restaurantRepository = restaurantRepository;
            _unitOfWork =unitOfWork;
        }


        public async Task<PagedResponse<PlatformRestaurantResponse>>GetRestaurantsAsync(PagedRequest request)
        {
            var totalItems =
                await _restaurantRepository.CountAsync();

            var restaurants =
                await _restaurantRepository.GetPagedAsync(
                    request.Page,
                    request.PageSize);

            var data = restaurants
                .Select(MapToResponse)
                .ToList();

            var totalPages =
                (int)Math.Ceiling(
                    (double)totalItems / request.PageSize);

            return new PagedResponse<PlatformRestaurantResponse>
            {
                Items = data,

                Pagination = new PaginationMeta
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages
                }
            };
        }


        public async Task<PlatformRestaurantResponse>GetRestaurantByIdAsync(Guid restaurantId)
        {
            var restaurant =
                await _restaurantRepository.GetByIdAsync(
                    restaurantId);

            if (restaurant == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy nhà hàng.");
            }

            return MapToResponse(restaurant);
        }


        public async Task<PlatformRestaurantResponse>CreateRestaurantAsync(CreateRestaurantRequest request)
        {
            var restaurant = new Restaurant
            {
                Name = request.Name.Trim(),

                Address = request.Address.Trim(),

                PhoneNumber = request.PhoneNumber.Trim(),

                Email = request.Email
                    .Trim()
                    .ToLowerInvariant(),

                Description =
                    request.Description?.Trim(),

                LogoUrl =
                    request.LogoUrl?.Trim(),

                IsActive = true
            };

            await _restaurantRepository.AddAsync(
                restaurant);

            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(restaurant);
        }


        public async Task<PlatformRestaurantResponse>UpdateRestaurantAsync(Guid restaurantId,UpdateRestaurantRequest request)
        {
            var restaurant =
                await _restaurantRepository.GetByIdAsync(
                    restaurantId);

            if (restaurant == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy nhà hàng.");
            }

            restaurant.Name =
                request.Name.Trim();

            restaurant.Address =
                request.Address.Trim();

            restaurant.PhoneNumber =
                request.PhoneNumber.Trim();

            restaurant.Email =
                request.Email
                    .Trim()
                    .ToLowerInvariant();

            restaurant.Description =
                request.Description?.Trim();

            restaurant.LogoUrl =
                request.LogoUrl?.Trim();

            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(restaurant);
        }


        public async Task ActivateRestaurantAsync(Guid restaurantId)
        {
            var restaurant =
                await _restaurantRepository.GetByIdAsync(
                    restaurantId);

            if (restaurant == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy nhà hàng.");
            }

            if (restaurant.IsActive)
            {
                throw new BadRequestException(
                    "Nhà hàng đang ở trạng thái hoạt động.");
            }

            restaurant.IsActive = true;

            await _unitOfWork.SaveChangesAsync();
        }


        public async Task DeactivateRestaurantAsync(Guid restaurantId)
        {
            var restaurant =
                await _restaurantRepository.GetByIdAsync(
                    restaurantId);

            if (restaurant == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy nhà hàng.");
            }

            if (!restaurant.IsActive)
            {
                throw new BadRequestException(
                    "Nhà hàng đã ở trạng thái ngừng hoạt động.");
            }

            restaurant.IsActive = false;

            await _unitOfWork.SaveChangesAsync();
        }


        private static PlatformRestaurantResponse MapToResponse(Restaurant restaurant)
        {
            return new PlatformRestaurantResponse
            {
                Id = restaurant.Id,

                Name = restaurant.Name,

                Address = restaurant.Address,

                PhoneNumber = restaurant.PhoneNumber,

                Email = restaurant.Email,

                Description = restaurant.Description,

                LogoUrl = restaurant.LogoUrl,

                IsActive = restaurant.IsActive,

                CreatedAt = restaurant.CreatedAt,

                UpdatedAt = restaurant.UpdatedAt
            };
        }
    }
}