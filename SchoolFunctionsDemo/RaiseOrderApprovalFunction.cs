
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using SchoolFunctionsDemo.SchoolFunctionsDemo;
using System.Net;

namespace SchoolFunctionsDemo;

public class RaiseOrderApprovalFunction
{
    [Function("RaiseOrderApproval")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "post",
            Route = "orders/approval")] HttpRequestData req,
        [DurableClient] DurableTaskClient durableClient)
    {
        var approval =
            await req.ReadFromJsonAsync<OrderApprovalRequest>();

        if (approval is null ||
            string.IsNullOrWhiteSpace(approval.InstanceId))
        {
            var badRequest =
                req.CreateResponse(HttpStatusCode.BadRequest);

            await badRequest.WriteStringAsync(
                "InstanceId and Approved are required.");

            return badRequest;
        }

        await durableClient.RaiseEventAsync(
            approval.InstanceId,
            "OrderApproval",
            approval.Approved);

        var response =
            req.CreateResponse(HttpStatusCode.Accepted);

        await response.WriteStringAsync(
            $"Approval event sent for instance {approval.InstanceId}.");

        return response;
    }
}
