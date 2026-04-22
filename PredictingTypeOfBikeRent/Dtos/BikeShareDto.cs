using Microsoft.ML.Data;

namespace PredictingTypeOfBikeRent.Dtos
{
    public class BikeShareDto
    {

        [LoadColumn(0)]
        public float Season { get; set; }

        [LoadColumn(1)]
        public float Month { get; set; }

        [LoadColumn(2)]
        public float Hour { get; set; }

        [LoadColumn(3)]
        public bool Holiday { get; set; }

        [LoadColumn(4)]
        public float WeekDay { get; set; }

        [LoadColumn(5)]
        public bool WorkingDay { get; set; }

        [LoadColumn(6)]
        public float WeatherCondition { get; set; }

        [LoadColumn(7)]
        public float Temperature { get; set; }

        [LoadColumn(8)]
        public float Humidity { get; set; }

        [LoadColumn(9)]
        public float WindSpeed { get; set; }

        // Целевая переменная        
        [LoadColumn(10)]
        public bool RentalTerm { get; set; }

    }
}
