using System.ComponentModel.DataAnnotations;
using static PredictingTypeOfBikeRent.Dtos.BikeShareEnumDto;

namespace PredictingTypeOfBikeRent.Dtos.OutputModels
{
    public class BikeShareOutputData
    {
        public SeasonType Season { get; set; }

        public MonthType Month { get; set; }

        public float Hour { get; set; }

        public bool Holiday { get; set; }

        public WeekDayType WeekDay { get; set; }

        public bool WorkingDay { get; set; }

        public WeatherConditionType WeatherCondition { get; set; }

        public float Temperature { get; set; }

        public float Humidity { get; set; }

        public float WindSpeed { get; set; }

        // Целевая переменная        
        [Required]
        public RentalType RentalTerm { get; set; }
    }
}
