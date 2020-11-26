using AutoMapper;
using DatingApp.API.Model;
using DatingApp.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public  AutoMapperProfiles()
        {

            // OUT
            CreateMap<User, UserForDetailed>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculatedAge()));

            CreateMap<User, UserForList>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculatedAge()));

            CreateMap<Photo, PhotoForDetailed>();
            CreateMap<Photo, PhotoForReturn>();
            // IN
            CreateMap<UserForUpdate, User>();
            CreateMap<PhotoForCreation, Photo>();

        }
    }
}
