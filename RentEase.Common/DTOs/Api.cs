using System.Net;

namespace RentEase.Common.DTOs
{
    public class ApiRes<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? Count { get; set; } = 0;
        public int? CurrentPage { get; set; } = 0;
        public int? TotalPages { get; set; } = 0;
        public T? Data { get; set; }
    }

    public class PagedResult<T>
    {
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    }
}
