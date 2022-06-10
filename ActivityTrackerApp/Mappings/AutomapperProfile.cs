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
            _mapActivity();
            _mapSession();
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
        private void _mapActivity()
        {
            CreateMap<Activity, ActivityDto>();
            CreateMap<Activity, ActivityPutDto>();
            CreateMap<Activity, ActivityPostDto>();

            CreateMap<ActivityDto, Activity>();
            CreateMap<ActivityPutDto, Activity>();
            CreateMap<ActivityPostDto, Activity>();
        }
        private void _mapSession()
        {
            CreateMap<Session, SessionDto>();
            CreateMap<Session, UserPutDto>();
            CreateMap<Session, UserPostDto>();

            CreateMap<SessionDto, Session>();
            CreateMap<SessionPutDto, Session>();
            CreateMap<SessionPostDto, Session>();
        }
    }
}