using AutoMapper;
using RedCrossChat.Entities;

namespace RedCrossChat
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, UserVm>().ReverseMap();

        }
    }
}
