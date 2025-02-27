namespace RentEase.Common.Base
{
    public class EnumType
    {
        public enum ApproveStatus
        {
            Pending = 1,      // Đang xét duyệt
            Approved = 2,     // Đã xét duyệt
            Rejected = 3      // Đã từ chối
        }

        public enum ProgressStatus
        {
            NotYet = 1,       // Chưa
            InProgress = 2,   // Trong quá trình
            Done = 3          // Hoàn thành
        }

        public enum TransactionStatus
        {
            Pending = 1,      // Đang xét duyệt
            Completed = 2,    // Hoàn thành
            Failed = 3        // Thất bại
        }

        public enum ContractStatus
        {
            Pending = 1,       // Đang xét duyệt
            Active = 2,        // Hoạt động
            ExpiringSoon = 3,  // Sắp hết hạn
            Completed = 4,     // Hoàn thành
            Cancelled = 5      // Hủy
        }

        public enum Priority
        {
            Low = 1,          // Thấp
            Medium = 2,       // Trung bình
            High = 3          // Cao
        }

        public enum Status
        {
            Active = 1,       // Đang hoạt động
            MoveOut = 2       // Rời đi
        }

    }
}
