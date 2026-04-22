using Microsoft.ML;
using Microsoft.ML.Data;
using PredictingTypeOfBikeRent.Dtos;
using PredictingTypeOfBikeRent.Dtos.OutputModels;

namespace PredictingTypeOfBikeRent.Evaluation
{
    public class ModelEvaluator
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        private readonly IDataView _testDataSet;

        public ModelEvaluator(MLContext mlContext, ITransformer model, IDataView testDataSet)
        {
            _mlContext = mlContext;
            _model = model;
            _testDataSet = testDataSet;

        }


        // Оценивает качество модели на тестовой выборке
        public BinaryClassificationMetrics? EvaluateModel()
        {   if( _testDataSet != null) {
                var predictions = _model.Transform(_testDataSet); // Получаем предсказания
                var metrics = _mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "RentalTerm");

            Console.WriteLine("*** Метрики качества модели ***");
            Console.WriteLine($"\tAccuracy: {metrics.Accuracy:F4}");
            Console.WriteLine($"\tAUC: {metrics.AreaUnderRocCurve:F4}");
            Console.WriteLine($"\tF1 Score: {metrics.F1Score:F4}");
            Console.WriteLine($"\tPositive Precision: {metrics.PositivePrecision:F4}");
            Console.WriteLine($"\tPositive Recall: {metrics.PositiveRecall:F4}");
            Console.WriteLine($"\tNegative Precision: {metrics.NegativePrecision:F4}");
            Console.WriteLine($"\tNegative Recall: {metrics.NegativeRecall:F4}");

            AnalyzeConfusionMatrix(predictions);
            return metrics;
            }
            else
            {
                Console.WriteLine("Не задана тестовая выборка данных");
                return null;
            }
        }

        // Анализирует матрицу ошибок
        private void AnalyzeConfusionMatrix(IDataView predictions)
        {
        
            var predictionData = _mlContext.Data.CreateEnumerable<BikeSharePredictionWithActual>(
                predictions, reuseRowObject: false).ToList();

            int tp = predictionData.Count(p => p.RentalTerm == true && p.PredictedLabel == true);
            int fp = predictionData.Count(p => p.RentalTerm == false && p.PredictedLabel == true);
            int tn = predictionData.Count(p => p.RentalTerm == false && p.PredictedLabel == false);
            int fn = predictionData.Count(p => p.RentalTerm == true && p.PredictedLabel == false);

            Console.WriteLine("\n*** Матрица ошибок ***");
            Console.WriteLine($"\tTrue Positive (TP): {tp}");
            Console.WriteLine($"\tFalse Positive (FP): {fp}");
            Console.WriteLine($"\tTrue Negative (TN): {tn}");
            Console.WriteLine($"\tFalse Negative (FN): {fn}");

            Console.WriteLine("\n      | Predicted |");
            Console.WriteLine("      | Short | Long |");
            Console.WriteLine("------|-------|------|");
            Console.WriteLine($"Actual Short |  {tn,-5} |  {fp,-5} |");
            Console.WriteLine($"Actual Long  |  {fn,-5} |  {tp,-5} |");
        }

    }

}