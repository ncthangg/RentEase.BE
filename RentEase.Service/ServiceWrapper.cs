using RentEase.Service.Service.Main;
using RentEase.Service.Service.Sub;

namespace RentEase.Service
{
    public class ServiceWrapper
    {
        public IAccountService AccountService { get; set; }
        public IAptCategoryService AptCategoryService { get; set; }
        public IAptImageService AptImageService { get; set; }
        public IAptService AptService { get; set; }
        public IAptStatusService AptStatusService { get; set; }
        public IAptUtilityService AptUtilityService { get; set; }
        public IUtilityService UtilityService { get; set; }
        public IReviewService ReviewService { get; set; }
        public IRoleService RoleService { get; set; }
        public ITransactionTypeService TransactionTypeService { get; set; }
        public ITransactionService TransactionService { get; set; }
        public IPostService PostService { get; set; }
        public IPostRequireService PostRequireService { get; set; }

        public ServiceWrapper(
             IAccountService accountService,
             IAptCategoryService aptCategoryService,
             IAptImageService aptImageService,
             IAptService aptService,
             IAptStatusService aptStatusService,
             IAptUtilityService aptUtilityService,
             IUtilityService utilityService,
             IReviewService reviewService,
             IRoleService roleService,
             ITransactionTypeService transactionTypeService,
             ITransactionService walletTransactionService,
             IPostService postService,
             IPostRequireService postRequireService
             )
        {
            AccountService = accountService;
            AptCategoryService = aptCategoryService;
            AptImageService = aptImageService;
            AptService = aptService;
            AptStatusService = aptStatusService;
            AptUtilityService = aptUtilityService;
            UtilityService = utilityService;
            ReviewService = reviewService;
            RoleService = roleService;
            TransactionTypeService = transactionTypeService;
            TransactionService = walletTransactionService;
            PostService = postService;
            PostRequireService = postRequireService;
        }
    }
}
