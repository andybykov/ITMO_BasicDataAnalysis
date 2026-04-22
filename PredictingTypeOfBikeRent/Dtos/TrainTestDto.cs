using Microsoft.ML;

namespace PredictingTypeOfBikeRent.Dtos
{
    // Класс для хранения разделенных обучающей и тестовой выборок
    public class TrainTestDto
    {
        public IDataView TrainSet { get; set; }
        public IDataView TestSet { get; set; }

        public TrainTestDto(IDataView trainSet, IDataView testSet)
        {
            TrainSet = trainSet;
            TestSet = testSet;
        }
    }
}
