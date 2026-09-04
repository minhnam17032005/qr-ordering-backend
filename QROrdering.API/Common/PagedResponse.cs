namespace QROrdering.API.Common
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public PaginationMeta Pagination { get; set; } = new();
    }
}
