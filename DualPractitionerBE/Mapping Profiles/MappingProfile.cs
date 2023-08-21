using AutoMapper;
using DAL.Models;
using DTO.DTOs;
using DTO.DTOs.PractitionersRegistry;
using DTO.DTOs.Responses;

namespace DualPractitionerBE.Mapping_Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<practitionerInfo, PractitionerDataV3>()
                .ForMember(s => s.full_name_ar, s => s.MapFrom(d => d.fullNameAr))
                .ForMember(s => s.full_name_en, s => s.MapFrom(d => d.fullNameEn))

                .ForPath(s => s.scfhs.specialty_name_ar, s => s.MapFrom(d => d.specialityAr))
                .ForPath(s => s.scfhs.specialty_name_en, s => s.MapFrom(d => d.specialityEn))
                .ForPath(s => s.scfhs.specialty_code, s => s.MapFrom(d => d.specialityCode))

                .ForPath(s => s.scfhs.registration_number, s => s.MapFrom(d => d.scfhsRegistrationNumber))
                .ForPath(s => s.scfhs.license_expiry_date, s => s.MapFrom(d => d.scfhsRegistrationExpiryDate))

                .ForMember(s => s.birth_date_hijri, s => s.MapFrom(d => d.birth_date_hijri))
                .ForMember(s => s.birth_date_gregorian, s => s.MapFrom(d => d.birth_date_gregorian))

                // We dont need license_number and license_expiry_date
                //.ForPath(s => s.hls_licenses[0].license_number, s => s.MapFrom(d => d.licenseNumber))
                //.ForPath(s => s.hls_licenses[0].license_expiry_date, s => s.MapFrom(d => d.licenseExpiryDate))

                .ForMember(s => s.gender_code, s => s.MapFrom(d => d.gender_code))

                .ForPath(s => s.scfhs.rank_name_ar, s => s.MapFrom(d => d.ScfhsCategoryAr))
                .ForPath(s => s.scfhs.rank_name_en, s => s.MapFrom(d => d.ScfhsCategoryEn))

                .ReverseMap();

            CreateMap<PrivateOrgReview, PaymentTransactionDetail>().ReverseMap();
            CreateMap<WorkDay, DaySchedule>().ReverseMap();

            //CreateMap<DAL.Models.MedicalOrganizationSubCategory, DTO.DTOs.Responses.MedicalOrganizationSubCategory>()
            //    .ForMember(s => s.Category.Id, s => s.MapFrom(d => d.CategoryId))
            //    .ReverseMap();

            CreateMap<Organization, OrganizationFullData>().ReverseMap();

        }
    }
}
