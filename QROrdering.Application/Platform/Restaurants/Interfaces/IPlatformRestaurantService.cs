using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Platform.Restaurants.DTOs.Requests;
using QROrdering.Application.Platform.Restaurants.DTOs.Responses;

namespace QROrdering.Application.Platform.Restaurants.Interfaces
{
    public interface IPlatformRestaurantService
    {
        Task<PagedResponse<PlatformRestaurantResponse>>GetRestaurantsAsync(PagedRequest request);

        Task<PlatformRestaurantResponse>GetRestaurantByIdAsync(Guid restaurantId);

        Task<PlatformRestaurantResponse>CreateRestaurantAsync(CreateRestaurantRequest request);

        Task<PlatformRestaurantResponse>UpdateRestaurantAsync(
                Guid restaurantId,
                UpdateRestaurantRequest request);

        Task ActivateRestaurantAsync(Guid restaurantId);

        Task DeactivateRestaurantAsync(Guid restaurantId);
    }
}
