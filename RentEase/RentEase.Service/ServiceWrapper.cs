using RentEase.Service.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public IContractService ContractService { get; set; }
        public ICurrentResidentService CurrentResidentService { get; set; }
        public IMaintenanceRequestService MaintenanceRequestService { get; set; }
        public IReviewService ReviewService { get; set; }
        public IRoleService RoleService { get; set; }
        public ITransactionTypeService TransactionTypeService { get; set; }
        public IWalletService WalletService { get; set; }
        public IWalletTransactionService WalletTransactionService { get; set; }


        public ServiceWrapper(
             IAccountService accountService,
             IAptCategoryService aptCategoryService,
             IAptImageService aptImageService,
             IAptService aptService,
             IAptStatusService aptStatusService,
             IAptUtilityService aptUtilityService,
             IUtilityService utilityService,
             IContractService contractService,
             ICurrentResidentService currentResidentService,
             IMaintenanceRequestService maintenanceRequestService,
             IReviewService reviewService,
             IRoleService roleService,
             ITransactionTypeService transactionTypeService,
             IWalletService walletService,
             IWalletTransactionService walletTransactionService

             )
        {
            AccountService = accountService;
            AptCategoryService = aptCategoryService;
            AptImageService = aptImageService;
            AptService = aptService;
            AptStatusService = aptStatusService;
            AptUtilityService = aptUtilityService;
            UtilityService = utilityService;
            ContractService = contractService;
            CurrentResidentService = currentResidentService;
            MaintenanceRequestService = maintenanceRequestService;
            ReviewService = reviewService;
            RoleService = roleService;
            TransactionTypeService = transactionTypeService;
            WalletService = walletService;
            WalletTransactionService = walletTransactionService;
                
                
        }
    }
}
