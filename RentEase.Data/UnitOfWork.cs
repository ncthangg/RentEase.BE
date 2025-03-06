

using RentEase.Data.DBContext;
using RentEase.Data.Repository.Base;
using RentEase.Data.Repository.Main;
using RentEase.Data.Repository.Sub;

namespace RentEase.Data
{
    public class UnitOfWork
    {
        private AccountRepository _accountRepository;
        private AccountTokenRepository _accountTokenRepository;
        private AccountVerificationRepository _accounVerificationRepository;
        private AptCategoryRepository _aptCategoryRepository;
        private AptImageRepository _aptImageRepository;
        private AptRepository _aptRepository;
        private AptStatusRepository _aptStatusRepository;
        private AptUtilityRepository _aptUtilityRepository;
        private UtilityRepository _utilityRepository;
        private ReviewRepository _reviewRepository;
        private RoleRepository _roleRepository;
        private TransactionTypeRepository _transactionTypeRepository;
        private OrderRepository _orderRepository;
        private TransactionRepository _transactionRepository;

        private PostRepository _postRepository;
        private PostRequireRepository _postRequireRepository;

        private RentEaseContext _dbContext;
        public UnitOfWork()
        {
            _dbContext ??= new RentEaseContext();
        }

        public AccountRepository AccountRepository => _accountRepository ??= new Repository.Main.AccountRepository(_dbContext);
        public AccountTokenRepository AccountTokenRepository => _accountTokenRepository ??= new Repository.Main.AccountTokenRepository(_dbContext);
        public AccountVerificationRepository AccountVerificationRepository => _accounVerificationRepository ??= new Repository.Main.AccountVerificationRepository(_dbContext);
        public AptCategoryRepository AptCategoryRepository => _aptCategoryRepository ??= new Repository.Sub.AptCategoryRepository(_dbContext);
        public AptImageRepository AptImageRepository => _aptImageRepository ??= new Repository.Main.AptImageRepository(_dbContext);
        public AptRepository AptRepository => _aptRepository ??= new Repository.Main.AptRepository(_dbContext);
        public AptStatusRepository AptStatusRepository => _aptStatusRepository ??= new Repository.Sub.AptStatusRepository(_dbContext);
        public AptUtilityRepository AptUtilityRepository => _aptUtilityRepository ??= new Repository.Main.AptUtilityRepository(_dbContext);
        public UtilityRepository UtilityRepository => _utilityRepository ??= new Repository.Sub.UtilityRepository(_dbContext);
        public ReviewRepository ReviewRepository => _reviewRepository ??= new Repository.Main.ReviewRepository(_dbContext);
        public RoleRepository RoleRepository => _roleRepository ??= new Repository.Sub.RoleRepository(_dbContext);
        public TransactionTypeRepository TransactionTypeRepository => _transactionTypeRepository ??= new Repository.Sub.TransactionTypeRepository(_dbContext);
        public OrderRepository OrderRepository => _orderRepository ??= new Repository.Main.OrderRepository(_dbContext);
        public TransactionRepository TransactionRepository => _transactionRepository ??= new Repository.Main.TransactionRepository(_dbContext);
        public PostRepository PostRepository => _postRepository ??= new Repository.Main.PostRepository(_dbContext);
        public PostRequireRepository PostRequireRepository => _postRequireRepository ??= new Repository.Main.PostRequireRepository(_dbContext);

        public GenericRepository<T> GetRepository<T>() where T : class
        {
            return new GenericRepository<T>();
        }
    }
}
