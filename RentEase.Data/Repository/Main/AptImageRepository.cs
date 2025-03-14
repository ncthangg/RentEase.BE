using Microsoft.EntityFrameworkCore;
using RentEase.Common.DTOs.Dto;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Main
{
    public class AptImageRepository : GenericRepository<AptImage>
    {
        public AptImageRepository()
        {
        }
        public AptImageRepository(RentEaseContext context) => _context = context;

        public async Task<AptImageRes> GetByAptIdAsync(string aptId)
        {
            var aptImages = await _context.AptImages
                                          .Where(x => x.AptId == aptId)
                                          .ToListAsync();

            if (!aptImages.Any())
                return null; // Không có ảnh, trả về null

            return new AptImageRes
            {
                AptId = aptId,
                Images = aptImages.Select(img => new Image
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    CreateAt = img.CreatedAt,
                    UpdateAt = img.UpdatedAt ?? img.CreatedAt
                }).ToList()
            };
        }

        //public async Task<PagedResult<Order>> GetOrdersForUserAsync(
        //       int accountId, int page = 1, int pageSize = 10)
        //{
        //    return await GetPagedAsync(
        //        filter: o => o.AccountId == accountId,
        //        orderBy: q => q.OrderByDescending(o => o.CreatedAt),
        //        page: page,
        //        pageSize: pageSize);
        //}
    }
}
