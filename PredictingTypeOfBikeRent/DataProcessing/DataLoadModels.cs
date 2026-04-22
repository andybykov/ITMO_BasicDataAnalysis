using Microsoft.ML;
using PredictingTypeOfBikeRent.Dtos;

namespace PredictingTypeOfBikeRent.DataProcessing
{
    // Работа с готовыми моделями
    public class DataLoadModels
    {
        private readonly MLContext _mlContext;

        public DataLoadModels(MLContext mlContext)
        {
            _mlContext = mlContext;
        }

        // загрузка из CSV
        public IDataView LoadDataFromCsv(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return null;
            }

            var data = _mlContext.Data.LoadFromTextFile<BikeShareDto>(
                path: filePath,
                separatorChar: ',',
                hasHeader: true);

            return data;
        }

        public ITransformer LoadModel(string modelPath)
        {
            if (!File.Exists(modelPath))
            {
                Console.WriteLine($"File not found: {modelPath}");
                return null;
            }

            return _mlContext.Model.Load(modelPath, out var modelSchema);
        }
    }
}
