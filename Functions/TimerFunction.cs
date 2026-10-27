//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.Functions.Worker.Extensions.Timer;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;

//namespace SchoolFunctionsDemo.Functions;

//public class TimerFunction
//{
//    private readonly ILogger<TimerFunction> _logger;
//    private readonly IConfiguration _configuration;

//    public TimerFunction(ILogger<TimerFunction> logger, IConfiguration configuration)
//    {
//        _logger = logger;
//        _configuration = configuration;
//    }

//    [Function("TimerFunction")]
//    public void Run([TimerTrigger("0 0 0 * * *")] TimerInfo timerInfo)
//    {
//        string? connectionString = _configuration["AzureWebJobsStorage2"];
//        string? connectionString2 = _configuration.GetConnectionString("AzureWebJobsStorage2");

//        if (string.IsNullOrWhiteSpace(connectionString))
//        {
//            _logger.LogError(
//                "AzureWebJobsStorage is not configured.");
//            return;
//        }

//        _logger.LogInformation(
//            "AzureWebJobsStorage configuration was loaded successfully.");

//        if (timerInfo.IsPastDue)
//        {
//            _logger.LogWarning("TimerFunction is running past due.");
//        }

//        int[] numbers =
//        [
//            12, 45, 7, 23, 98, 31, 45, 64
//        ];

//        int highest = int.MinValue;
//        int secondHighest = int.MinValue;

//        foreach (int number in numbers)
//        {
//            if (number > highest)
//            {
//                secondHighest = highest;
//                highest = number;
//            }
//            else if (number > secondHighest && number < highest)
//            {
//                secondHighest = number;
//            }
//        }

//        _logger.LogInformation("Array values: {Numbers}", string.Join(", ", numbers));

//        if (secondHighest == int.MinValue)
//        {
//            _logger.LogWarning("A second highest distinct number was not found.");
//            return;
//        }

//        _logger.LogInformation("Highest number: {Highest}", highest);
//        _logger.LogInformation("Second highest number: {SecondHighest}", secondHighest);
//    }
//}