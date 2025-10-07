using AutoMapper;
using DatingApp.Dtos;
using DatingApp.Entities;

namespace DatingApp.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<SeedUserDto, Member>();

        CreateMap<SeedUserDto, AppUser>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.Member, opt => opt.MapFrom(src => src));
    }
}