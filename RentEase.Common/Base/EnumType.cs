namespace RentEase.Common.Base
{
    public class EnumType
    {
        public enum Role
        {
            Admin = 1,
            Lessor = 2,
            Lesses = 3
        }

        public enum GenderId
        {
            Male = 1,
            Female = 2,
            Other = 3
        }

        public enum OldId
        {
            Age18To22 = 1,
            Age22To26 = 2,
            Age26To30 = 3,
            Age30To40 = 4
        }

        public enum AptStatusId
        {
            AVAILABLE = 1,
            UNAVAILABLE = 2
        }

        public enum ApproveStatusId
        {
            PENDING = 1,      // Đang xử lý
            SUCCESS = 2,      // Thành công
            FAILED = 3        // Thất bại
        }

        public enum PaymentStatusId
        {
            PENDING = 1,   // Chờ thanh toán
            PAID = 2,      // Đã thanh toán
            PROCESSING = 3,      // Đang xử lý
            CANCELLED = 4        // Hủy
        }

        public enum PostCategoryId
        {
            THUENHA = 1,
            KIEMBANCUNGPHONG = 2,
        }

    }
}
