using Microsoft.AspNetCore.Mvc;
using QROrdering.API.Common;

namespace QROrdering.API.Extensions
{
    public static class ControllerExtensions
    {
        // Tạo response thành công 200 OK
        public static ActionResult<ApiResponse<T>> ApiOk<T>(
            this ControllerBase controller,
            T data,
            string message = "Success")
        {
            return controller.Ok(
                new ApiResponse<T>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = message,
                    Data = data
                });
        }

        // Tạo response thành công 201 Created
        public static ActionResult<ApiResponse<T>> ApiCreated<T>(
            this ControllerBase controller,
            T data,
            string message = "Created successfully")
        {
            return controller.StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<T>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status201Created,
                    Message = message,
                    Data = data
                });
        }

        public static IActionResult ApiNoContent(
            this ControllerBase controller)
        {
            return controller.NoContent();
        }
    }
}
