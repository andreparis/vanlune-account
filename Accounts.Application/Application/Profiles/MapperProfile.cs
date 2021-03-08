using Accounts.Domain.DTO;
using Accounts.Domain.Entities;
using AutoMapper;

namespace Accounts.Application.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
