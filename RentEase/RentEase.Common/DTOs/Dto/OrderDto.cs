using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentEase.Common.DTOs.Dto
{
    public class RequestOrderDto
    {
        public int ContractId { get; set; }

        public int LessorId { get; set; }

        public int LesseeId { get; set; }

        public decimal Amount { get; set; } = 0;

        public int TransactionTypeId { get; set; }

        public DateTime DueDate { get; set; }
    }
    public class ResponseOrderDto
    {
        public string Id { get; set; }

        public int ContractId { get; set; }

        public int LessorId { get; set; }

        public int LesseeId { get; set; }

        public decimal Amount { get; set; }

        public int TransactionTypeId { get; set; }

        public int TransactionStatusId { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
