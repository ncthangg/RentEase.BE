using Microsoft.EntityFrameworkCore;
using RentEase.Common.DTOs;
using RentEase.Common.DTOs.Dto;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Main
{
    public class AptUtilityRepository : GenericRepository<AptUtility>
    {
        public AptUtilityRepository()
        {
        }
        public AptUtilityRepository(RentEaseContext context) => _context = context;
        public async Task<PagedResult<AptUtility>> GetByAptId(
                string aptId, int page = 1, int pageSize = 10)
        {
            return await GetPagedAsync(
                filter: (f => f.AptId == aptId),
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize,
                includes: q => q.Include(i => i.Utility));
        }
        public async Task<IEnumerable<AptUtility>?> GetByAptId(string aptId)
        {
            var aptImages = await _context.AptUtilities
                                          .Where(x => x.AptId == aptId)
                                          .ToListAsync();

            if (!aptImages.Any())
                return null; // Không có ảnh, trả về null

            return aptImages;
        }

        public async Task<bool> RemoveAsync(string aptId, int utilityId)
        {
            var entity = await _context.AptUtilities
            .FirstOrDefaultAsync(x => x.AptId == aptId && x.UtilityId == utilityId);

            if (entity == null)
                return false;

            _context.AptUtilities.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveAllAsync(string aptId)
        {
            var entities = _context.AptUtilities.Where(x => x.AptId == aptId);
            _context.AptUtilities.RemoveRange(entities);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
