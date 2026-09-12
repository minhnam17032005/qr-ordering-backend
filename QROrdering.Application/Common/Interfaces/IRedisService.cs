using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QROrdering.Application.Common.Interfaces
{
    // CRUD và quản lý dữ liệu với Redis
    public interface IRedisService
    {
        /// <summary>
        /// Lưu dữ liệu vào Redis.
        /// </summary>
        Task<bool> SetAsync<T>(
            string key,
            T value,
            TimeSpan? expiry = null);

        /// <summary>
        /// Lấy dữ liệu từ Redis.
        /// </summary>
        Task<T?> GetAsync<T>(
            string key);

        /// <summary>
        /// Kiểm tra key có tồn tại không.
        /// </summary>
        Task<bool> ExistsAsync(
            string key);

        /// <summary>
        /// Xóa một hoặc nhiều key.
        /// </summary>
        Task<long> RemoveAsync(
            params string[] keys);

        /// <summary>
        /// Lấy thời gian sống còn lại của key.
        /// </summary>
        Task<TimeSpan?> GetTimeToLiveAsync(
            string key);

        /// <summary>
        /// Gia hạn thời gian sống của key.
        /// </summary>
        Task<bool> ExpireAsync(
            string key,
            TimeSpan expiry);

        /// <summary>
        /// Xóa tất cả key theo pattern.
        /// </summary>
        Task RemoveByPatternAsync(
            string pattern);

        /// <summary>
        /// Lấy danh sách key theo pattern.
        /// </summary>
        IEnumerable<string> GetKeys(
            string pattern);
    }
}