using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using AutoMapper;

namespace ActivityTrackerApp.Mappings;

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
        CreateMap<Activity, ActivityGetDto>();
        CreateMap<Activity, ActivityUpdateDto>();
        CreateMap<Activity, ActivityCreateDto>();

        CreateMap<ActivityGetDto, Activity>();
        CreateMap<ActivityUpdateDto, Activity>();
        CreateMap<ActivityCreateDto, Activity>();
    }

    private void _mapSession()
    {
        CreateMap<Session, SessionGetDto>();
        CreateMap<Session, SessionUpdateDto>();
        CreateMap<Session, SessionCreateDto>();

        CreateMap<SessionGetDto, Session>();
        CreateMap<SessionUpdateDto, Session>();
        CreateMap<SessionCreateDto, Session>();
    }
}