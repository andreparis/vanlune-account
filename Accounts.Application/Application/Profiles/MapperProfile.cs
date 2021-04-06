using Accounts.Domain.DTO;
using Accounts.Domain.Entities;
using AutoMapper;
using System.Linq;

namespace Accounts.Application.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role, 
                opts => opts.MapFrom(src => src.Roles.Any() && src.Roles.First().Role != null ? src.Roles.First().Role.Name : ""))
                .ForMember(dest => dest.RoleId, 
                opts => opts.MapFrom(src => src.Roles.Any() && src.Roles.First().Role != null ? src.Roles.First().Role.Id : 0))
                .ReverseMap();
        }
    }
}
