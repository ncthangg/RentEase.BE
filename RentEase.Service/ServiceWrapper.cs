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
        public IAptUtilityService AptUtilityService { get; set; }
        public IUtilityService UtilityService { get; set; }
        public IReviewService ReviewService { get; set; }
        public IRoleService RoleService { get; set; }
        public IOrderService OrderService { get; set; }
        public IOrderTypeService OrderTypeService { get; set; }
        public IPostService PostService { get; set; }
        public IPostRequireService PostRequireService { get; set; }

        public ServiceWrapper(
             IAccountService accountService,
             IAptCategoryService aptCategoryService,
             IAptImageService aptImageService,
             IAptService aptService,
             IAptUtilityService aptUtilityService,
             IUtilityService utilityService,
             IReviewService reviewService,
             IRoleService roleService,
             IOrderService orderService,
             IOrderTypeService orderTypeService,
             IPostService postService,
             IPostRequireService postRequireService
             )
        {
            AccountService = accountService;
            AptCategoryService = aptCategoryService;
            AptImageService = aptImageService;
            AptService = aptService;
            AptUtilityService = aptUtilityService;
            UtilityService = utilityService;
            ReviewService = reviewService;
            RoleService = roleService;
            OrderService = orderService;
            OrderTypeService = orderTypeService;
            PostService = postService;
            PostRequireService = postRequireService;
        }
    }
}
