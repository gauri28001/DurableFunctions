//using Microsoft.Azure.Functions.Worker;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;

//namespace SchoolFunctionsDemo.Functions;

//public class HelloFunction
//{
//    private readonly ILogger<HelloFunction> _logger;

//    public HelloFunction(ILogger<HelloFunction> logger)
//    {
//        _logger = logger;
//    }

//    [Function("HelloFunction")]
//    public IActionResult Run(
//        [HttpTrigger( AuthorizationLevel.Function, "get","post",Route = "hello")] HttpRequest req)
//    {
//        _logger.LogInformation("HelloFunction execution started.");

//        string? name = req.Query["name"].FirstOrDefault();

//        if (string.IsNullOrWhiteSpace(name))
//        {
//            _logger.LogWarning( "Request rejected because the name is missing.");

//            return new BadRequestObjectResult(new
//            {
//                error = "Please provide a name.",
//                example = "/api/hello?name=Gauri"
//            });
//        }

//        _logger.LogInformation( "Processing greeting for {Name}", name);

//        var result = new
//        {
//            message = $"Hello, {name}! Welcome to Azure Functions.",
//            functionName = "HelloFunction",
//            timestamp = DateTimeOffset.UtcNow
//        };

//        _logger.LogInformation( "HelloFunction execution completed successfully.");

//        return new OkObjectResult(result);
//    }
//}