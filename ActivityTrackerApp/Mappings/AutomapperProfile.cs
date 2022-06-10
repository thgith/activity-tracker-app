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
            CreateMap<User, UserGetDto>();
            CreateMap<User, UserUpdateDto>();
            CreateMap<User, UserLoginDto>();
            CreateMap<User, UserRegisterDto>();

            CreateMap<UserGetDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<UserLoginDto, User>();
            CreateMap<UserRegisterDto, User>();
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
            CreateMap<Session, SessionPutDto>();
            CreateMap<Session, SessionPostDto>();

            CreateMap<SessionDto, Session>();
            CreateMap<SessionPutDto, Session>();
            CreateMap<SessionPostDto, Session>();
        }
    }
}