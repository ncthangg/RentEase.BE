using RentEase.Data.DBContext;
using RentEase.Data.Repository;
using RentEase.Data.Repository.Base;

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
        private ContractRepository _contractRepository;
        private CurrentResidentRepository _currentResidentRepository;
        private MaintenanceRequestRepository _maintenanceRequestRepository;
        private ReviewRepository _reviewRepository;
        private RoleRepository _roleRepository;
        private TransactionTypeRepository _transactionTypeRepository;
        private WalletRepository _walletRepository;
        private WalletTransactionRepository _walletTransactionRepository;

        private RentEaseContext _dbContext;
        public UnitOfWork()
        {
            _dbContext ??= new RentEaseContext(); 
        }

        public AccountRepository AccountRepository => _accountRepository ??= new Repository.AccountRepository(_dbContext);
        public AccountTokenRepository AccountTokenRepository => _accountTokenRepository ??= new Repository.AccountTokenRepository(_dbContext);
        public AccountVerificationRepository AccountVerificationRepository => _accounVerificationRepository ??= new Repository.AccountVerificationRepository(_dbContext);
        public AptCategoryRepository AptCategoryRepository => _aptCategoryRepository ??= new Repository.AptCategoryRepository(_dbContext);
        public AptImageRepository AptImageRepository => _aptImageRepository ??= new Repository.AptImageRepository(_dbContext);
        public AptRepository AptRepository => _aptRepository ??= new Repository.AptRepository(_dbContext);
        public AptStatusRepository AptStatusRepository => _aptStatusRepository ??= new Repository.AptStatusRepository(_dbContext);
        public AptUtilityRepository AptUtilityRepository => _aptUtilityRepository ??= new Repository.AptUtilityRepository(_dbContext);
        public UtilityRepository UtilityRepository => _utilityRepository ??= new Repository.UtilityRepository(_dbContext);
        public ContractRepository ContractRepository => _contractRepository ??= new Repository.ContractRepository(_dbContext);
        public CurrentResidentRepository CurrentResidentRepository => _currentResidentRepository ??= new Repository.CurrentResidentRepository(_dbContext);
        public MaintenanceRequestRepository MaintenanceRequestRepository => _maintenanceRequestRepository ??= new Repository.MaintenanceRequestRepository(_dbContext);
        public ReviewRepository ReviewRepository => _reviewRepository ??= new Repository.ReviewRepository(_dbContext);
        public RoleRepository RoleRepository => _roleRepository ??= new Repository.RoleRepository(_dbContext);
        public TransactionTypeRepository TransactionTypeRepository => _transactionTypeRepository ??= new Repository.TransactionTypeRepository(_dbContext);
        public WalletRepository WalletRepository => _walletRepository ??= new Repository.WalletRepository(_dbContext);
        public WalletTransactionRepository WalletTransactionRepository => _walletTransactionRepository ??= new Repository.WalletTransactionRepository(_dbContext);

        public GenericRepository<T> GetRepository<T>() where T : class
        {
            return new GenericRepository<T>();
        }
    }
}
