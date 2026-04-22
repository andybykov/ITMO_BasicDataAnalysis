using AutoMapper;
using static PredictingTypeOfBikeRent.Dtos.BikeShareEnumDto;
using PredictingTypeOfBikeRent.Dtos;
using PredictingTypeOfBikeRent.Dtos.OutputModels;
using PredictingTypeOfBikeRent.Dtos.InputModels;

namespace PredictingTypeOfBikeRent.MappingProfiles
{
    public class BikeShareMappingProfile : Profile
    {
        public BikeShareMappingProfile()
        {
            // Dto → Output
            CreateMap<BikeShareDto, BikeShareOutputData>()
    .ForMember(dest => dest.Season,
        opt => opt.MapFrom(src => (SeasonType)src.Season)) // явно приводим к enum
    .ForMember(dest => dest.WeatherCondition,
        opt => opt.MapFrom(src => (WeatherConditionType)src.WeatherCondition))
    .ForMember(dest => dest.RentalTerm,
        opt => opt.MapFrom(src => src.RentalTerm
            // явная логика преобразования в bool
            ? RentalType.LongTerm
            : RentalType.ShortTerm)) 
    .ForMember(dest => dest.Month,
        opt => opt.MapFrom(src => (MonthType)src.Month))
    .ForMember(dest => dest.WeekDay,
        opt => opt.MapFrom(src => (WeekDayType)src.WeekDay));

            //
            // InputData → Dto
            CreateMap<BikeShareInputData, BikeShareDto>()
                .ForMember(dest => dest.Season,
                    opt => opt.MapFrom(src => (float)src.Season)) // enum в float
                .ForMember(dest => dest.Month,
                    opt => opt.MapFrom(src => (float)src.Month)) // enum в float
                .ForMember(dest => dest.WeekDay,
                    opt => opt.MapFrom(src => (float)src.WeekDay)) // enum в float
                .ForMember(dest => dest.WeatherCondition,
                    opt => opt.MapFrom(src => (float)src.WeatherCondition)) // enum в float
                 // Остальные поля совпадают по именам и типам
                .ForMember(dest => dest.RentalTerm, opt => opt.Ignore());   // целевая переменная не нужна

        }
    }
}