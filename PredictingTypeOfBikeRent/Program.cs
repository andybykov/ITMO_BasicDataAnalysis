using Microsoft.ML;
using PredictingTypeOfBikeRent.DataProcessing;
using PredictingTypeOfBikeRent.Dtos;
using PredictingTypeOfBikeRent.Evaluation;
using PredictingTypeOfBikeRent.ModelTraining;
using PredictingTypeOfBikeRent.PredictionEngine;


class Program
{
    // Путь к файлам данных
    private static string _pathToProjcet = @"D:\VS_Projects\ML.NET\PredictingTypeOfBikeRent";
    private static string _folderData = "Data";
    private static FileInfo _dataFile = new FileInfo(Path.Combine(_pathToProjcet, _folderData, "bike_sharing.csv"));
    private static FileInfo _trainDataSetFile = new FileInfo(Path.Combine(_pathToProjcet, _folderData, "test_data_set.csv"));



    static void Main(string[] args)
    {
        // Создание ML.NET контекста с фиксированным seed для воспроизводимости результатов
        var mlContext = new MLContext(seed: 22);

        try
        {
            FileInfo _modelFullPath = new FileInfo(Path.Combine(_pathToProjcet, "LearningModels", "bike_rent_model.zip")); // путь к модели
            if (!_dataFile.Exists) throw new FileNotFoundException("Data file in not exists!");
            // если мы не нашли по пути модель
            if (!File.Exists(_modelFullPath.FullName))
            {

                // Шаг 1: Загрузка и анализ данных
                Console.WriteLine("Загрузка и анализ исходных данных...");
                var dataProcessor = new DataProcessor(mlContext);
                var data = dataProcessor.LoadData(_dataFile.FullName);

                // Анализ и вывод информации об исходных данных
                dataProcessor.ExploreData(data);

                // Шаг 2: Разделение данных 
                Console.WriteLine("\nРазделение данных и создание пайплайна обработки...");
                TrainTestDto trainTestData = dataProcessor.SplitData(data);
                // создание пайплайна
                var dataPrepPipeline = dataProcessor.CreateDataProcessingPipeline();

                // Шаг 3: Обучение модели  и выбор лучшей
                Console.WriteLine("\nОбучение модели...");
                var modelTrainer = new ModelTrainer(mlContext, trainTestData.TrainSet, trainTestData.TestSet, dataPrepPipeline);
                var bestModel = modelTrainer.TrainAndCompareModels();

                // Шаг 4: Оценка качества модели на тестовой выборке
                var predicion = new PredictionEngine(mlContext, bestModel);
                predicion.DemonstratePredictions();

                // Шаг 5: Оцениваем качество модели
                var evaluator = new ModelEvaluator(mlContext, bestModel, trainTestData.TestSet);
                var metrics = evaluator.EvaluateModel();

                // Сохранени модели
                Console.WriteLine("\nСохранение модели...");
                modelTrainer.SaveModel(_modelFullPath.FullName, bestModel);

                // Сохранение тествовый выборки для дальнейшей оценки
                modelTrainer.SaveTestDataSetCsv(_trainDataSetFile.FullName);
            }
            else
            {
                var loadModels = new DataLoadModels(mlContext);

                // Загружаем тестовые данные
                var trainData = loadModels.LoadDataFromCsv(_trainDataSetFile.FullName);
                if (trainData == null)
                {
                    throw new FileNotFoundException("Test data file in not exists!");
                }

                var model = loadModels.LoadModel(_modelFullPath.FullName);

                // Шаг 5: Оцениваем качество модели
                var evaluator = new ModelEvaluator(mlContext, model, trainData);
                var metrics = evaluator.EvaluateModel();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nОшибка: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }

        Console.WriteLine("\nНажмите любую клавишу для завершения...");
        Console.ReadKey();
    }
}