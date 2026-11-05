using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Common.Pagination
{
    public class PagedRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page phải lớn hơn hoặc bằng 1.")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize phải nằm trong khoảng từ 1 đến 100.")]
        public int PageSize { get; set; } = 10;

        public string? SortBy { get; set; }

        public bool SortDescending { get; set; }

        public string? Search { get; set; }
    }
}
