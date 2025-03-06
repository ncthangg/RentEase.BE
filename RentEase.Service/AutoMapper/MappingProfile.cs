using AutoMapper;
using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;
using RentEase.Data.Models;

namespace RentEase.Service.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Login
            CreateMap<AccountToken, AccountTokenRes>();
            CreateMap<AccountVerification, AccountVerificationRes>();

            CreateMap<Account, AccountReq>().ReverseMap();
            CreateMap<Account, AccountRes>().ReverseMap()
                    .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender ?? null))
                    .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl ?? null));


            CreateMap<Apt, AptRes>().ReverseMap();
            CreateMap<AptCategory, AptCategoryRes>().ReverseMap();
            CreateMap<AptImage, AptImageRes>().ReverseMap();
            CreateMap<AptStatus, AptStatusRes>().ReverseMap();
            CreateMap<AptUtility, AptUtilityRes>().ReverseMap();
            CreateMap<Contract, ContractRes>().ReverseMap();
            CreateMap<CurrentResident, CurrentResidentRes>().ReverseMap();
            CreateMap<MaintenanceRequest, MaintenanceRequestRes>().ReverseMap();
            CreateMap<Review, ReviewRes>().ReverseMap();
            CreateMap<Role, RoleRes>().ReverseMap();
            CreateMap<TransactionType, TransactionTypeRes>().ReverseMap();
            CreateMap<Utility, UtilityRes>().ReverseMap();
            CreateMap<Order, OrderRes>().ReverseMap();
            CreateMap<Wallet, WalletRes>().ReverseMap();
            CreateMap<Transaction, TransactionRes>().ReverseMap();
        }
    }
}

