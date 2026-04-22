using Microsoft.ML.Data;

namespace PredictingTypeOfBikeRent.Dtos.OutputModels
{
    // Класс представления результатов предсказания
    public class BikeSharePredictionData
    {
        // предсказанное значение
        [ColumnName("PredictionLable")]
        public bool PredictedRentalType { get; set; }

        // Вероятность
        public float Probability { get; set; }

        // Значение оценки перед преобразованием в вероятность
        public float Score { get; set; }
    }

    // Класс для совместного хранения фактических и предсказанных значений
    public class BikeSharePredictionWithActual
    {
        public bool RentalTerm { get; set; }          // факт
        public bool PredictedLabel { get; set; }      // предсказание
        public float Probability { get; set; }        // вероятность
        public float Score { get; set; }              // скор
    }
}
