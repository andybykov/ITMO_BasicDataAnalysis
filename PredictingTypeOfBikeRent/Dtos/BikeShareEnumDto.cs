using System;
using System.Collections.Generic;
using System.Text;

namespace PredictingTypeOfBikeRent.Dtos
{
    public static class BikeShareEnumDto
    {
        public enum SeasonType
        {
            // (1=зима, 2=весна, 3=лето, 4=осень)
            Winter = 1,
            Spring = 2,
            Summer = 3,
            Autumn = 4
        }

        public enum MonthType
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }

        public enum WeekDayType
        {
            Sunday = 0,
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6
        }

        public enum WeatherConditionType
        {
            // (1=ясно, 2=туман, 3=легкий дождь/снег, 4=сильный дождь/снег)             
            Clear = 1,
            Fog = 2,
            LightRain = 3,
            StrongRain = 4
        }

        public enum RentalType
        {
            // (0=short term, 1=long term)            
            ShortTerm = 0,
            LongTerm = 1
        }
    }
}
