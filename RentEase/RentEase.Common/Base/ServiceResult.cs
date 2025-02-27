namespace RentEase.Common.Base
{
    public interface IServiceResult
    {
        int Status { get; set; }
        string? Message { get; set; }
        object? Data { get; set; }
        int? TotalCount { get; set; }
        int? TotalPage { get; set; }
        int? CurrentPage { get; set; }
    }

    public class ServiceResult : IServiceResult
    {
        public int Status { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPage { get; set; }
        public int? CurrentPage { get; set; }

        public ServiceResult()
        {
            Status = -1;
            Message = "Action fail";
        }

        public ServiceResult(int status, string message)
        {
            Status = status;
            Message = message;
        }
        public ServiceResult(int status, string message, object data)
        {
            Status = status;
            Message = message;
            Data = data;
        }

        public ServiceResult(int status, string message, int totalCount, int totalPage, int currentPage, object data)
        {
            Status = status;
            Message = message;
            TotalCount = totalCount;
            TotalPage = totalPage;
            CurrentPage = currentPage;
            Data = data;
        }

    }
}
