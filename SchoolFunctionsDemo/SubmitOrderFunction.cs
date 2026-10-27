
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using System.Net;

namespace SchoolFunctionsDemo.SchoolFunctionsDemo;

public class SubmitOrderFunction
{
    private readonly ILogger<SubmitOrderFunction> _logger;

    public SubmitOrderFunction(
        ILogger<SubmitOrderFunction> logger)
    {
        _logger = logger;
    }

    [Function("SubmitOrder")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")]
        HttpRequestData req, [DurableClient] DurableTaskClient durableClient)
    {
        var order = await req.ReadFromJsonAsync<OrderRequest>();

        if (order is null || string.IsNullOrWhiteSpace(order.OrderId) || string.IsNullOrWhiteSpace(order.ProductId)
            || order.Quantity <= 0 || order.Amount <= 0)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid order request.");
            return badRequest;
        }

        string instanceId = await durableClient.ScheduleNewOrchestrationInstanceAsync(nameof(OrderOrchestrator), order);

        _logger.LogInformation("Started order {OrderId}. Instance ID: {InstanceId}", order.OrderId, instanceId);

        return await durableClient.CreateCheckStatusResponseAsync(req, instanceId);
    }
}
