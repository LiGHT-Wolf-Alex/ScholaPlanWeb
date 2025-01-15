using AutoMapper;
using ScholaPlan.API.DTOs;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.API.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TeacherPreferencesDto, TeacherPreferences>()
            .ForMember(dest => dest.AvailableDays,
                opt => opt.MapFrom(src => src.AvailableDays.Select(d => (DayOfWeek)d).ToList()))
            .ForMember(dest => dest.AvailableLessonNumbers, opt => opt.MapFrom(src => src.AvailableLessonNumbers))
            .ForMember(dest => dest.PreferredRoomIds, opt => opt.MapFrom(src => src.PreferredRoomIds));
    }
}