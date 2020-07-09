using AutoMapper;
using DatingAppApi.Models;
using DatingAppApi.Models.DTOs;
using DatingAppApi.Models.ViewModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppApi.Helper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {

            CreateMap<User, UserListVM>().ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).URL))
                                                     .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<User, UserForDetailsVM>().ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).URL))
                                               .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge())); 
            CreateMap<Photo, PhotoForDetailedVM>();
            CreateMap<UserForUpdateVM, User>();
            CreateMap<Photo, PhotoFromReturnVM>();
            CreateMap<PhotoforCreationVM, Photo>();
            CreateMap<UserVM, User>();

        }
    }
}
