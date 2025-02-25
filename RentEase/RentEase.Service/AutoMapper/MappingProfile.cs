using AutoMapper;
using RentEase.Common.DTOs.Dto;
using RentEase.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RentEase.Service.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Account, ResponseAccountDto>()
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
            CreateMap<Wallet, ResponseWalletDto>();
            CreateMap<WalletTransaction, ResponseWalletTransactionDto>();
        }
    }
}

