
//using System.Text.Json;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Extensions.Logging;

//namespace SchoolFunctionsDemo.Functions;

//public class FindDuplicateIntegersFunction
//{
//    private readonly ILogger<FindDuplicateIntegersFunction> _logger;

//    public FindDuplicateIntegersFunction(
//        ILogger<FindDuplicateIntegersFunction> logger)
//    {
//        _logger = logger;
//    }

//    [Function(nameof(FindDuplicateIntegersFunction))]
//    public void Run(
//        [ServiceBusTrigger(
//            "integer-arrays",
//            Connection = "ServiceBusConnection")] string message)
//    {
//        int[]? numbers;

//        try
//        {
//            numbers = JsonSerializer.Deserialize<int[]>(message);
//        }
//        catch (JsonException ex)
//        {
//            _logger.LogError(ex, "Message is not a valid JSON integer array.");
//            throw;
//        }

//        if (numbers is null || numbers.Length == 0)
//        {
//            _logger.LogWarning("Received an empty or null integer array.");
//            return;
//        }

//        var frequencies = new Dictionary<int, int>();

//        foreach (int number in numbers)
//        {
//            frequencies[number] = frequencies.GetValueOrDefault(number) + 1;
//        }

//        int maxCount = frequencies.Values.Max();

//        var mostFrequent = frequencies
//            .Where(item => item.Value == maxCount)
//            .Select(item => item.Key)
//            .OrderBy(number => number)
//            .ToArray();

//        if (maxCount == 1)
//        {
//            _logger.LogInformation(
//                "No duplicates found. Maximum frequency is 1.");
//            return;
//        }

//        _logger.LogInformation(
//            "Maximum frequency: {MaxCount}. Numbers: {Numbers}",
//            maxCount,
//            string.Join(", ", mostFrequent));
//    }
//}
