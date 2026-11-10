using Microsoft.Extensions.Options;
using QROrdering.Application.Common.Configurations;
using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Exceptions;
using QROrdering.Application.RestaurantManagement.Tables.DTOs.Requests;
using QROrdering.Application.RestaurantManagement.Tables.DTOs.Responses;
using QROrdering.Application.RestaurantManagement.Tables.Interfaces;
using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Domain.Enums;

namespace QROrdering.Application.RestaurantManagement.Tables
{
    public class RestaurantTableService : IRestaurantTableService
    {
        private readonly IRestaurantTableRepository _tableRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;

        public RestaurantTableService(
            IRestaurantTableRepository tableRepository,
            IUnitOfWork unitOfWork,
            IOptions<AppSettings> appSettings)
        {
            _tableRepository = tableRepository;
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
        }

        public async Task<PagedResponse<TableListResponse>> GetTablesAsync(
            Guid restaurantId,
            PagedRequest request)
        {
            if (restaurantId == Guid.Empty)
            {
                throw new BadRequestException("RestaurantId không hợp lệ.");
            }

            var restaurantExists =
                await _tableRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
            {
                throw new NotFoundException("Không tìm thấy nhà hàng.");
            }

            var totalItems =
                await _tableRepository.CountByRestaurantAsync(restaurantId);

            var tables =
                await _tableRepository.GetPagedByRestaurantAsync(
                    restaurantId,
                    request);

            return new PagedResponse<TableListResponse>
            {
                Items = tables,
                Pagination = new PaginationMeta
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling(
                        (double)totalItems / request.PageSize)
                }
            };
        }

        public async Task<TableDetailResponse> GetTableByIdAsync(
            Guid restaurantId,
            Guid tableId)
        {
            if (restaurantId == Guid.Empty || tableId == Guid.Empty)
            {
                throw new BadRequestException(
                    "Thông tin định danh không hợp lệ.");
            }

            var restaurantExists =
                await _tableRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
            {
                throw new NotFoundException("Không tìm thấy nhà hàng.");
            }

            var table = await _tableRepository.GetByIdInRestaurantAsync(
                restaurantId,
                tableId);

            if (table is null)
            {
                throw new NotFoundException("Không tìm thấy bàn.");
            }

            return table;
        }

        public async Task<TableDetailResponse> CreateTableAsync(
            Guid restaurantId,
            TableCreateRequest request)
        {
            // 1. Validate input.
            if (restaurantId == Guid.Empty)
            {
                throw new BadRequestException("RestaurantId không hợp lệ.");
            }

            if (request.TableNumber <= 0)
            {
                throw new BadRequestException("Số bàn phải lớn hơn 0.");
            }

            // 2. Check restaurant existence.
            var restaurantExists =
                await _tableRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
            {
                throw new NotFoundException("Không tìm thấy nhà hàng.");
            }

            // 3. Check duplicate table number.
            var tableNumberExists =
                await _tableRepository.ExistsByTableNumberAsync(
                    restaurantId,
                    request.TableNumber);

            if (tableNumberExists)
            {
                throw new ConflictException("Số bàn đã tồn tại.");
            }

            // 4. Validate application configuration.
            var frontendBaseUrl = _appSettings.FrontendBaseUrl?.Trim();

            if (string.IsNullOrWhiteSpace(frontendBaseUrl) ||
                !Uri.TryCreate(frontendBaseUrl, UriKind.Absolute, out var frontendUri) ||
                (frontendUri.Scheme != Uri.UriSchemeHttp &&
                 frontendUri.Scheme != Uri.UriSchemeHttps))
            {
                throw new InvalidOperationException(
                    "FrontendBaseUrl configuration không hợp lệ.");
            }

            frontendBaseUrl = frontendBaseUrl.TrimEnd('/');

            // 5. Initialize table entity.
            var table = new RestaurantTable
            {
                RestaurantId = restaurantId,
                TableNumber = request.TableNumber,
                QRCode = string.Empty,
                Status = TableStatus.Available,
                IsActive = true
            };

            // 6. Create table and generate QRCode in transaction.
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                await _tableRepository.AddAsync(table);

                // Save lần 1: SQL Server sinh Id bằng NEWSEQUENTIALID().
                await _unitOfWork.SaveChangesAsync();

                // Save lần 2: Cập nhật QRCode dựa trên Id vừa sinh.
                table.QRCode =
                    $"{frontendBaseUrl}/r/{restaurantId}/table/{table.Id}";

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            // 7. Return created table.
            var response = await _tableRepository.GetByIdInRestaurantAsync(
                restaurantId,
                table.Id);

            if (response is null)
            {
                throw new NotFoundException(
                    "Không tìm thấy bàn sau khi tạo.");
            }

            return response;
        }

        public async Task<TableDetailResponse> UpdateTableAsync(
            Guid restaurantId,
            Guid tableId,
            TableUpdateRequest request)
        {
            if (restaurantId == Guid.Empty || tableId == Guid.Empty)
            {
                throw new BadRequestException(
                    "Thông tin định danh không hợp lệ.");
            }

            if (request.TableNumber <= 0)
            {
                throw new BadRequestException("Số bàn phải lớn hơn 0.");
            }

            var restaurantExists =
                await _tableRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
            {
                throw new NotFoundException("Không tìm thấy nhà hàng.");
            }

            var table =
                await _tableRepository.GetEntityByIdInRestaurantAsync(
                    restaurantId,
                    tableId);

            if (table is null)
            {
                throw new NotFoundException("Không tìm thấy bàn.");
            }

            var tableNumberExists =
                await _tableRepository.ExistsByTableNumberAsync(
                    restaurantId,
                    request.TableNumber,
                    tableId);

            if (tableNumberExists)
            {
                throw new BadRequestException("Số bàn đã tồn tại.");
            }

            table.TableNumber = request.TableNumber;
            table.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            var response = await _tableRepository.GetByIdInRestaurantAsync(
                restaurantId,
                tableId);

            if (response is null)
            {
                throw new NotFoundException(
                    "Không tìm thấy bàn sau khi cập nhật.");
            }

            return response;
        }

        public async Task<TableStatusResponse> ActivateTableAsync(
            Guid restaurantId,
            Guid tableId)
        {
            if (restaurantId == Guid.Empty || tableId == Guid.Empty)
            {
                throw new BadRequestException(
                    "Thông tin định danh không hợp lệ.");
            }

            var restaurantExists =
                await _tableRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
            {
                throw new NotFoundException("Không tìm thấy nhà hàng.");
            }

            var table =
                await _tableRepository.GetEntityByIdInRestaurantAsync(
                    restaurantId,
                    tableId);

            if (table is null)
            {
                throw new NotFoundException("Không tìm thấy bàn.");
            }

            if (table.IsActive)
            {
                throw new BadRequestException("Bàn đã được kích hoạt.");
            }

            table.IsActive = true;
            table.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return new TableStatusResponse
            {
                Id = table.Id,
                IsActive = table.IsActive
            };
        }

        public async Task<TableStatusResponse> DeactivateTableAsync(
            Guid restaurantId,
            Guid tableId)
        {
            if (restaurantId == Guid.Empty || tableId == Guid.Empty)
            {
                throw new BadRequestException(
                    "Thông tin định danh không hợp lệ.");
            }

            var restaurantExists =
                await _tableRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
            {
                throw new NotFoundException("Không tìm thấy nhà hàng.");
            }

            var table =
                await _tableRepository.GetEntityByIdInRestaurantAsync(
                    restaurantId,
                    tableId);

            if (table is null)
            {
                throw new NotFoundException("Không tìm thấy bàn.");
            }

            if (!table.IsActive)
            {
                throw new BadRequestException("Bàn đã bị vô hiệu hóa.");
            }

            if (table.Status == TableStatus.Occupied)
            {
                throw new BadRequestException(
                    "Không thể vô hiệu hóa bàn đang được sử dụng.");
            }

            table.IsActive = false;
            table.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return new TableStatusResponse
            {
                Id = table.Id,
                IsActive = table.IsActive
            };
        }
    }
}