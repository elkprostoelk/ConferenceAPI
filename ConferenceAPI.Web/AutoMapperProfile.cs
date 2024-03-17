using AutoMapper;
using ConferenceAPI.Core.DTO;
using ConferenceAPI.Web.Models;

namespace ConferenceAPI.Web
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateZoomMeetingSettingsModel, CreateZoomMeetingSettingsDto>();
            CreateMap<CreateZoomMeetingModel, CreateZoomMeetingDto>()
                .ForMember(dto => dto.Settings, opts => opts.MapFrom(m => m.Settings));
        }
    }
}
