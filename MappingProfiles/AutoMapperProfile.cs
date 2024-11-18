using AutoMapper;
using SeminarIntegration.DTOs;

namespace SeminarIntegration.MappingProfiles;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<NewUserRequest, User>()
            .ForMember(dest => dest.Uuid, opt => opt.Ignore())
            // .ForMember(
            //     dest => dest.Username,
            //     opt => opt.MapFrom(src => src.Username.ToLower())
            // )
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.FullName, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.PreviouslyDeleted, opt => opt.Ignore())
            ;

        // for responses
        CreateMap<User, NormalUserResponse>();
        CreateMap<User, ElevatedNormalUserResponse>();

        CreateMap<UpdateUserRequest, User>()
            .ForMember(dest => dest.FirstName, opt => opt.Condition(src => src.FirstName != null))
            .ForMember(dest => dest.LastName, opt => opt.Condition(src => src.LastName != null))
            .ForMember(dest => dest.Title, opt => opt.Condition(src => src.Title != null))
            ;
    }
}