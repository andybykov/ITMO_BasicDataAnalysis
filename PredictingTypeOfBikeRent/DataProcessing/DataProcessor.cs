using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Data;
using PredictingTypeOfBikeRent.Dtos;
using PredictingTypeOfBikeRent.Dtos.OutputModels;
using static PredictingTypeOfBikeRent.Dtos.BikeShareEnumDto;
using PredictingTypeOfBikeRent.MappingProfiles;

namespace PredictingTypeOfBikeRent.DataProcessing
{
    public class DataProcessor
    {
        private readonly MLContext _mlContext;
        private Mapper _mapper;

        public DataProcessor(MLContext mlContext)
        {
            _mlContext = mlContext;

            // Создаем MapperConfiguration и передаем BikeShareMappingProfile
            MapperConfiguration configuration = new MapperConfiguration((cfg =>
            {
                cfg.AddProfile(new BikeShareMappingProfile());
            }), new LoggerFactory());

            _mapper = new Mapper(configuration);
        }


        // Загружает данные из CSV-файла
        public IDataView LoadData(string dataPath)
        {
            // Загрузка данных с учетом особенностей формата
            var data = _mlContext.Data.LoadFromTextFile<BikeShareDto>(
                path: dataPath,
                hasHeader: true,
                separatorChar: ',',
                allowQuoting: true,
                trimWhitespace: true);

            Console.WriteLine($"Данные загружены из файла: {dataPath}");
            return data;
        }

        // Разделяет данные на обучающую и тестовую выборки
        public TrainTestDto SplitData(IDataView data)
        {
            var mlSplitData = _mlContext.Data.TrainTestSplit(data, testFraction: 0.2);
            var trainTestData = new TrainTestDto(mlSplitData.TrainSet, mlSplitData.TestSet);
            Console.WriteLine("Данные разделены на обучающую и тестовую выборки (80% / 20%)");
            return trainTestData;
        }


        // Выполняет исследовательский анализ данных
        public void ExploreData(IDataView data)
        {
            var dataList = _mlContext.Data
                .CreateEnumerable<BikeShareDto>(data, reuseRowObject: false)
                .ToList();

            var mappedData = _mapper.Map<List<BikeShareOutputData>>(dataList);
            int total = mappedData.Count;
            Console.WriteLine($"Количество записей: {total}");

            // Категориальные признаки 
            Console.WriteLine("\nКатегориальные признаки:");

            // Season
            Console.WriteLine("\nSeason:");
            foreach (var season in Enum.GetValues<SeasonType>())
            {
                int count = mappedData.Count(x => x.Season == season);
                Console.WriteLine($"  {season} : {100.0 * count / total:F2}%");
            }

            // Month 
            Console.WriteLine("\nMonth:");
            foreach (var month in Enum.GetValues<MonthType>())
            {
                int count = mappedData.Count(x => x.Month == month);
                if (count > 0)
                    Console.WriteLine($"  {month} : {100.0 * count / total:F2}%");
            }

            // WeekDay
            Console.WriteLine("\nWeekDay:");
            foreach (var day in Enum.GetValues<WeekDayType>())
            {
                int count = mappedData.Count(x => x.WeekDay == day);
                if (count > 0)
                    Console.WriteLine($"  {day} : {100.0 * count / total:F2}%");
            }

            // WeatherCondition
            Console.WriteLine("\nWeatherCondition:");
            foreach (var weather in Enum.GetValues<WeatherConditionType>())
            {
                int count = mappedData.Count(x => x.WeatherCondition == weather);
                Console.WriteLine($"  {weather} {100.0 * count / total:F2}%");
            }
            // Holiday 
            Console.WriteLine("\nHoliday:");
            int holidayTrue = mappedData.Count(x => x.Holiday);
            Console.WriteLine($"  True:  {100.0 * holidayTrue / total:F2}%");
            Console.WriteLine($"  False: {100.0 * (total - holidayTrue) / total:F2}%");

            // WorkingDay 
            Console.WriteLine("\nWorkingDay:");
            int workingTrue = mappedData.Count(x => x.WorkingDay);
            Console.WriteLine($"  True:  {100.0 * workingTrue / total:F2}%");
            Console.WriteLine($"  False: {100.0 * (total - workingTrue) / total:F2}%");

            Console.WriteLine("\nЧисловые признаки:");

            // Hour
            Console.WriteLine($"Hour: Min={mappedData.Min(x => x.Hour)}, Max={mappedData.Max(x => x.Hour)}, Avg={mappedData.Average(x => x.Hour):F2}," +
                $" Median={GetMedian(mappedData.Select(x => (float)x.Hour)):F2}");

            // Temperature
            Console.WriteLine($"Temperature: Min={mappedData.Min(x => x.Temperature):F2}, Max={mappedData.Max(x => x.Temperature):F2}, Avg={mappedData.Average(x => x.Temperature):F2}," +
                $" Median={GetMedian(mappedData.Select(x => x.Temperature)):F2}");

            // Humidity
            Console.WriteLine($"Humidity: Min={mappedData.Min(x => x.Humidity)}, Max={mappedData.Max(x => x.Humidity)}, Avg={mappedData.Average(x => x.Humidity):F2}," +
                $" Median={GetMedian(mappedData.Select(x => (float)x.Humidity)):F2}");

            // WindSpeed
            Console.WriteLine($"WindSpeed: Min={mappedData.Min(x => x.WindSpeed):F2}, Max={mappedData.Max(x => x.WindSpeed):F2}, Avg={mappedData.Average(x => x.WindSpeed):F2}," +
                $" Median={GetMedian(mappedData.Select(x => x.WindSpeed)):F2}");

            // Целевая переменная RentalTerm
            Console.WriteLine("\nЦелевая переменная RentalTerm:");
            foreach (var term in Enum.GetValues<RentalType>())
            {
                int count = mappedData.Count(x => x.RentalTerm == term);
                Console.WriteLine($"  {term}: {100.0 * count / total:F2}%");
            }
        }

        // Вспомогательный метод для медианы
        private float GetMedian(IEnumerable<float> values)
        {
            var sorted = values.OrderBy(x => x).ToList();
            int count = sorted.Count;
            if (count % 2 == 0)
                return (sorted[count / 2 - 1] + sorted[count / 2]) / 2;
            else
                return sorted[count / 2];
        }

        // Создание pipline для предобработки
        // превращает сырые данные в пригодный для обучения вектор признаков Features и метку Label

        public IEstimator<ITransformer> CreateDataProcessingPipeline()
        {
            Console.WriteLine("Создание пайплайна обработки данных для BikeShare...");

            // Преобразование целевой переменной bool в ключ 0/1 для бинарной классификации

            // var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(
            // inputColumnName: nameof(BikeShareDto.RentalTerm),
            //outputColumnName: "Label")

            //  Нормализация числовых признаков
            // Температура, влажность, скорость ветра и час имеют разные шкалы
            var pipeline = _mlContext.Transforms.NormalizeMinMax(
            inputColumnName: nameof(BikeShareDto.Temperature),
            outputColumnName: "TemperatureNorm")
                .Append(_mlContext.Transforms.NormalizeMinMax(
                    inputColumnName: nameof(BikeShareDto.Humidity),
                    outputColumnName: "HumidityNorm"))
                .Append(_mlContext.Transforms.NormalizeMinMax(
                    inputColumnName: nameof(BikeShareDto.WindSpeed),
                    outputColumnName: "WindSpeedNorm"))
                .Append(_mlContext.Transforms.NormalizeMinMax(
                    inputColumnName: nameof(BikeShareDto.Hour),
                    outputColumnName: "HourNorm"))

                //  One‑Hot Encoding категориальных признаков 
                // Сезон (1..4)
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(
                    inputColumnName: nameof(BikeShareDto.Season),
                    outputColumnName: "SeasonEncoded"))
                // Месяц (1..12)
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(
                    inputColumnName: nameof(BikeShareDto.Month),
                    outputColumnName: "MonthEncoded"))
                // День недели (0..6)
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(
                    inputColumnName: nameof(BikeShareDto.WeekDay),
                    outputColumnName: "WeekDayEncoded"))
                // Погодное условие (1..4)
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(
                    inputColumnName: nameof(BikeShareDto.WeatherCondition),
                    outputColumnName: "WeatherEncoded"))

                // Бинарные признаки в float 
                // Holiday и WorkingDay преобразуем из bool во float 0/1
                .Append(_mlContext.Transforms.Conversion.ConvertType(
                    inputColumnName: nameof(BikeShareDto.Holiday),
                    outputColumnName: "HolidayFloat",
                    outputKind: DataKind.Single))
                .Append(_mlContext.Transforms.Conversion.ConvertType(
                    inputColumnName: nameof(BikeShareDto.WorkingDay),
                    outputColumnName: "WorkingDayFloat",
                    outputKind: DataKind.Single))

                // Объединение всех признаков в один вектор
                .Append(_mlContext.Transforms.Concatenate("Features",
                    "TemperatureNorm",
                    "HumidityNorm",
                    "WindSpeedNorm",
                    "HourNorm",
                    "SeasonEncoded",
                    "MonthEncoded",
                    "WeekDayEncoded",
                    "WeatherEncoded",
                    "HolidayFloat",
                    "WorkingDayFloat"));

            Console.WriteLine("Пайплайн обработки данных создан");
            return pipeline;
        }

    }
}
