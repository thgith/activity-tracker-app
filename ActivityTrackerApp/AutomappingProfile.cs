using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using AutoMapper;

namespace ActivityTrackerApp.Mappings
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            _mapUser();
        }

        private void _mapUser()
        {
            CreateMap<User, UserDto>();
            CreateMap<User, UserPutDto>();
            CreateMap<User, UserPostDto>();

            CreateMap<UserDto, User>();
            CreateMap<UserPutDto, User>();
            CreateMap<UserPostDto, User>();
        }
    }
}