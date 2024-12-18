﻿using AutoMapper;
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
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.PreviouslyDeleted, opt => opt.Ignore())
            ;

        // for responses
        CreateMap<User, NormalUserResponse>();
        CreateMap<User, ElevatedNormalUserResponse>();

        CreateMap<UpdateUserRequest, User>();

        // Add mapping for SeminarRegistrationRespItem
        CreateMap<SemRegistrationODataItem, SeminarRegistrationRespItem>()
            .ForMember(dest => dest.SeminarNo, opt => opt.MapFrom(src => src.Seminar_No))
            .ForMember(dest => dest.SeminarName, opt => opt.MapFrom(src => src.Seminar_Name))
            .ForMember(dest => dest.StartingDate, opt => opt.MapFrom(src => src.Starting_Date))
            .ForMember(dest => dest.LineNo, opt => opt.MapFrom(src => src.Line_No))
            .ForMember(dest => dest.HeaderNo, opt => opt.MapFrom(src => src.Header_No))
            .ForMember(dest => dest.CompanyNo, opt => opt.MapFrom(src => src.Bill_to_Customer_No))
            .ForMember(dest => dest.ParticipantContactNo, opt => opt.MapFrom(src => src.Participant_Contact_No))
            .ForMember(dest => dest.ParticipantName, opt => opt.MapFrom(src => src.Participant_Name))
            .ForMember(dest => dest.ToInvoice, opt => opt.MapFrom(src => src.To_Invoice))
            .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.Registration_Date))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.ConfirmationStatus, opt => opt.MapFrom(src => src.Confirmation_Status))
            .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.Discount_Amount));
    }
}