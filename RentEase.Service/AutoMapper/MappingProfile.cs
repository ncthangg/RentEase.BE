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


            CreateMap<Apt, ResponseAptDto>().ReverseMap();
            CreateMap<AptCategory, ResponseAptCategoryDto>().ReverseMap();
            CreateMap<AptImage, ResponseAptImageDto>().ReverseMap();
            CreateMap<AptStatus, ResponseAptStatusDto>().ReverseMap();
            CreateMap<AptUtility, ResponseAptUtilityDto>().ReverseMap();
            CreateMap<Contract, ResponseContractDto>().ReverseMap();
            CreateMap<CurrentResident, ResponseCurrentResidentDto>().ReverseMap();
            CreateMap<MaintenanceRequest, ResponseMaintenanceRequestDto>().ReverseMap();
            CreateMap<Review, ResponseReviewDto>().ReverseMap();
            CreateMap<Role, ResponseRoleDto>().ReverseMap();
            CreateMap<TransactionType, ResponseTransactionTypeDto>().ReverseMap();
            CreateMap<Utility, ResponseUtilityDto>().ReverseMap();
            CreateMap<Order, ResponseOrderDto>().ReverseMap();
            CreateMap<Wallet, ResponseWalletDto>().ReverseMap();
            CreateMap<WalletTransaction, ResponseWalletTransactionDto>().ReverseMap();
        }
    }
}

