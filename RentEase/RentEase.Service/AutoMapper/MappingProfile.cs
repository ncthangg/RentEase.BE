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
            CreateMap<AccountToken, ResponseAccountToken>();
            CreateMap<AccountVerification, ResponseAccountVerification>();

            CreateMap<Account, RequestAccountDto>().ReverseMap();
            CreateMap<Account, ResponseAccountDto>().ReverseMap()
                    .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender ?? null))
                    .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl ?? null));


            CreateMap<Apt, ResponseAptDto>();
            CreateMap<AptCategory, ResponseAptCategoryDto>();
            CreateMap<AptImage, ResponseAptImageDto>();
            CreateMap<AptStatus, ResponseAptStatusDto>();
            CreateMap<AptUtility, ResponseAptUtilityDto>();
            CreateMap<Contract, ResponseContractDto>();
            CreateMap<CurrentResident, ResponseCurrentResidentDto>();
            CreateMap<MaintenanceRequest, ResponseMaintenanceRequestDto>();
            CreateMap<Review, ResponseReviewDto>();
            CreateMap<Role, ResponseRoleDto>();
            CreateMap<TransactionType, ResponseTransactionTypeDto>();
            CreateMap<Utility, ResponseUtilityDto>();
            CreateMap<Order, ResponseOrderDto>();
            CreateMap<Wallet, ResponseWalletDto>();
            CreateMap<WalletTransaction, ResponseWalletTransactionDto>();
        }
    }
}

