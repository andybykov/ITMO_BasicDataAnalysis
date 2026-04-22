using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using PredictingTypeOfBikeRent.Dtos;
using PredictingTypeOfBikeRent.Dtos.InputModels;
using PredictingTypeOfBikeRent.Dtos.OutputModels;
using PredictingTypeOfBikeRent.MappingProfiles;
using static PredictingTypeOfBikeRent.Dtos.BikeShareEnumDto;

namespace PredictingTypeOfBikeRent.PredictionEngine
{
    public class PredictionEngine
    {
        private readonly MLContext _mlContext;
        private ITransformer? _loadedModel;  // Добавляем ? для nullable
        private PredictionEngine<BikeShareDto, BikeSharePredictionData>? _predictionEngine;  // Добавляем ? для nullable
        private Mapper _mapper;

        public PredictionEngine(MLContext mlContext)
        {
            _mlContext = mlContext;
            // Создаем MapperConfiguration и передаем BikeShareMappingProfile
            MapperConfiguration configuration = new MapperConfiguration((cfg =>
            {
                cfg.AddProfile(new BikeShareMappingProfile());
            }), new LoggerFactory());

            _mapper = new Mapper(configuration);
        }


        public PredictionEngine(MLContext mlContext, ITransformer model)
        {
            _mlContext = mlContext;
            _loadedModel = model;
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<BikeShareDto, BikeSharePredictionData>(_loadedModel);
            // Создаем MapperConfiguration и передаем BikeShareMappingProfile
            MapperConfiguration configuration = new MapperConfiguration((cfg =>
            {
                cfg.AddProfile(new BikeShareMappingProfile());
            }), new LoggerFactory());

            _mapper = new Mapper(configuration);
        }

        // Загружает модель из файла
        public void LoadModel(string modelPath)
        {
            if (!File.Exists(modelPath))
            {
                throw new FileNotFoundException($"Файл модели не найден: {modelPath}");
            }

            // Загрузка модели
            DataViewSchema modelSchema;
            _loadedModel = _mlContext.Model.Load(modelPath, out modelSchema);

            // Создание движка прогнозирования
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<BikeShareDto, BikeSharePredictionData>(_loadedModel);

            Console.WriteLine($"Модель загружена из файла: {modelPath}");
        }

        // Возвращает загруженную модель
        public ITransformer GetLoadedModel()
        {
            if (_loadedModel != null)
            {
                return _loadedModel;
            }
            else return null;
        }


        // Демонстрирует прогнозирование на предопределенных примерах
        public void DemonstratePredictions()
        {
            if (_predictionEngine == null)
            {
                throw new InvalidOperationException("PredictionEngine не инициализирован");
            }

            Console.WriteLine("Демонстрация предсказаний на типовых примерах:");

            // Пример 1
            var example1 = new BikeShareInputData
            {
                Season = SeasonType.Winter,
                Month = MonthType.December,
                Hour = 23f,
                Holiday = false,
                WeekDay = WeekDayType.Monday,
                WorkingDay = true,
                WeatherCondition = WeatherConditionType.StrongRain,
                Temperature = 1.1f,
                Humidity = 66.5f,
                WindSpeed = 20.1f
            };

            var example1Mapped = _mapper.Map<BikeShareDto>(example1);
            var prediction1 = _predictionEngine?.Predict(example1Mapped);
   
            Console.WriteLine("\nПример 1: Зима, ветер, холодно");
            Console.WriteLine("Характеристики:");
            Console.WriteLine("Сезон: Зима");
            Console.WriteLine("Месяц: Декабрь");
            Console.WriteLine("Врямя позднее: 23:00");
            Console.WriteLine("День недели: понедельник");
            Console.WriteLine("Выходной: нет");
            Console.WriteLine("Погодные условия: сильный снег");
            Console.WriteLine("Температруа: 1,1 градус Цельсия");
            Console.WriteLine("Влажность: 66,5%");
            Console.WriteLine("Скорость ветра: 20,1");
            Console.WriteLine($"Предсказание будет ли аренда долгосрочной: {prediction1?.PredictedRentalType}");
            Console.WriteLine($"Вероятность долгострочной аренды: {prediction1?.Probability:P2}");
            Console.WriteLine($"Значение оценки: {prediction1?.Score:P2}");

            // Пример 2: Летний выходной день, ясно, тепло (ожидается долгосрочная аренда)
            var example2 = new BikeShareInputData
            {
                Season = SeasonType.Summer,
                Month = MonthType.July,
                Hour = 14f,
                Holiday = true,
                WeekDay = WeekDayType.Saturday,
                WorkingDay = false,
                WeatherCondition = WeatherConditionType.Clear,
                Temperature = 28.5f,
                Humidity = 55.0f,
                WindSpeed = 8.0f
            };

            var example2Mapped = _mapper.Map<BikeShareDto>(example2);
            var prediction2 = _predictionEngine?.Predict(example2Mapped);

            Console.WriteLine("\nПример 2: Лето, выходной, солнечно");
            Console.WriteLine("Характеристики:");
            Console.WriteLine("Сезон: Лето");
            Console.WriteLine("Месяц: Июль");
            Console.WriteLine("Время: 14:00");
            Console.WriteLine("День недели: суббота");
            Console.WriteLine("Выходной: да");
            Console.WriteLine("Погодные условия: ясно");
            Console.WriteLine("Температура: 28,5 градуса Цельсия");
            Console.WriteLine("Влажность: 55%");
            Console.WriteLine("Скорость ветра: 8");
            Console.WriteLine($"Предсказание будет ли аренда долгосрочной: {prediction2?.PredictedRentalType}");
            Console.WriteLine($"Вероятность долгосрочной аренды: {prediction2?.Probability:P2}");
            Console.WriteLine($"Значение оценки: {prediction2?.Score:P2}");

            // Пример 3: Утро рабочего дня, весна, легкий дождь (ожидается краткосрочная аренда)
            var example3 = new BikeShareInputData
            {
                Season = SeasonType.Spring,
                Month = MonthType.April,
                Hour = 8f,
                Holiday = false,
                WeekDay = WeekDayType.Tuesday,
                WorkingDay = true,
                WeatherCondition = WeatherConditionType.LightRain,
                Temperature = 10.0f,
                Humidity = 82.0f,
                WindSpeed = 12.0f
            };

            var example3Mapped = _mapper.Map<BikeShareDto>(example3);
            var prediction3 = _predictionEngine?.Predict(example3Mapped);

            Console.WriteLine("\nПример 3: Весеннее утро, рабочий день, небольшой дождь");
            Console.WriteLine("Характеристики:");
            Console.WriteLine("Сезон: Весна");
            Console.WriteLine("Месяц: Апрель");
            Console.WriteLine("Время: 8:00");
            Console.WriteLine("День недели: вторник");
            Console.WriteLine("Выходной: нет");
            Console.WriteLine("Погодные условия: легкий дождь/снег");
            Console.WriteLine("Температура: 10,0 градусов Цельсия");
            Console.WriteLine("Влажность: 82%");
            Console.WriteLine("Скорость ветра: 12");
            Console.WriteLine($"Предсказание будет ли аренда долгосрочной: {prediction3?.PredictedRentalType}");
            Console.WriteLine($"Вероятность долгосрочной аренды: {prediction3?.Probability:P2}");
            Console.WriteLine($"Значение оценки: {prediction3?.Score:P2}");

            // Пример 4: Осень, вечер пятницы, туман, умеренная погода (вероятна краткосрочная)
            var example4 = new BikeShareInputData
            {
                Season = SeasonType.Autumn,
                Month = MonthType.October,
                Hour = 18f,
                Holiday = false,
                WeekDay = WeekDayType.Friday,
                WorkingDay = true,
                WeatherCondition = WeatherConditionType.Fog,
                Temperature = 12.0f,
                Humidity = 88.0f,
                WindSpeed = 5.0f
            };

            var example4Mapped = _mapper.Map<BikeShareDto>(example4);
            var prediction4 = _predictionEngine?.Predict(example4Mapped);

            Console.WriteLine("\nПример 4: Осень, вечер пятницы, туман");
            Console.WriteLine("Характеристики:");
            Console.WriteLine("Сезон: Осень");
            Console.WriteLine("Месяц: Октябрь");
            Console.WriteLine("Время: 18:00");
            Console.WriteLine("День недели: пятница");
            Console.WriteLine("Выходной: нет (но близко)");
            Console.WriteLine("Погодные условия: туман");
            Console.WriteLine("Температура: 12,0 градусов Цельсия");
            Console.WriteLine("Влажность: 88%");
            Console.WriteLine("Скорость ветра: 5");
            Console.WriteLine($"Предсказание будет ли аренда долгосрочной: {prediction4?.PredictedRentalType}");
            Console.WriteLine($"Вероятность долгосрочной аренды: {prediction4?.Probability:P2}");
            Console.WriteLine($"Значение оценки: {prediction4?.Score:P2}");
        }

    }
}
