using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RentEase.Common.DTOs.Response
{
    public class ApiResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public int? Count { get; set; }
        public int? CurrentPage { get; set; }
        public int? TotalPages { get; set; }
        public T? Data { get; set; }
    }

    public class PagedResult<T>
    {
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
