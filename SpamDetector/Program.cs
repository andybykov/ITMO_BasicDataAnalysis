using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;
using Plotly.NET;
using SpamDetector;

public class Program
{
    private static string header = """
                            *** Spam email detecor ***

        |--------------------------------------------------------------------|
        |     Trainer                          MacroAccuracy    Duration     |
        |--------------------------------------------------------------------|
        |     TextClassificationMulti          0,9239           11,7480      |
        |--------------------------------------------------------------------|

        """;

    static void Main(string[] args)
    {
        Console.WriteLine(header);
        Console.WriteLine("*** В обучени модели применен тренер многоклассовой классификации текста, в данном случае - это бинарная классификация." +
            "\n*** В зависимости от текста почтового сообщения, модель предсказывает вероятность - спам/не спам.");

        try
        {   
            while (true)
            {
                Console.WriteLine("\nЖелаете протестировать модель? (y/n) ");
                char input = char.ToLower(Console.ReadKey().KeyChar);
                if (input == 'y' || input == 'д')
                {
                    Console.Write("\nВведите текст сообщения: ");
                    string msg = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(msg))
                    {
                        Console.WriteLine("Сообщение не может быть пустым!");
                        continue;
                    }

                    //Load sample data
                    var sampleData = new MLTextSpamModel.ModelInput()
                    {
                        //Content = @"Hi team, Just a reminder that we have our weekly project status meeting tomorrow at 10 AM in Conference Room
                        //          B. Please bring your status reports and be prepared to discuss the timeline updates. Thanks, Sarah",
                        Content = msg
                    };
                    //Load model and predict output
                    var result = MLTextSpamModel.Predict(sampleData);
                    var predictedLabel = result.PredictedLabel;

                    bool isSpam = predictedLabel == "1" ? true : false;
                    string message = isSpam ? "Это спам" : "Это не спам";
                    Console.WriteLine(message);

                    Console.WriteLine($"Уверенность модели (Score): {result.Score.Max():F2} ({result.Score.Max():P0})");
                    Console.WriteLine($"Вероятность спама: {result.Score.Min():P0}");
                }
                else
                {
                    break;
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }
}
