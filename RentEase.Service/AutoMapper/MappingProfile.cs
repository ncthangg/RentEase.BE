﻿using AutoMapper;
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
            CreateMap<AccountToken, AccountTokenRes>().ReverseMap();
            CreateMap<AccountVerification, AccountVerificationRes>().ReverseMap();

            CreateMap<Account, AccountReq>().ReverseMap();
            CreateMap<Account, AccountRes>().ReverseMap();


            CreateMap<Apt, AptRes>().ReverseMap();
            CreateMap<AptCategory, AptCategoryRes>().ReverseMap();
            CreateMap<AptStatus, AptStatusRes>().ReverseMap();
            CreateMap<AptUtility, AptUtilityRes>().ReverseMap();
            CreateMap<Review, ReviewRes>().ReverseMap();
            CreateMap<Role, RoleRes>().ReverseMap();
            CreateMap<TransactionType, TransactionTypeRes>().ReverseMap();
            CreateMap<Utility, UtilityRes>().ReverseMap();
            CreateMap<Order, OrderRes>().ReverseMap();
            CreateMap<Transaction, TransactionRes>().ReverseMap();

            CreateMap<Post, PostRes>().ReverseMap();
            CreateMap<PostRequire, PostRequireRes>().ReverseMap();

        }
    }
}

