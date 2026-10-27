
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using SchoolFunctionsDemo.SchoolFunctionsDemo;

namespace SchoolFunctionsDemo.SchoolFunctionsDemo;

public class OrderOrchestrator
{
    [Function(nameof(OrderOrchestrator))]
    public static async Task<OrderResult> Run(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var order = context.GetInput<OrderRequest>();

        if (order is null)
        {
            return new OrderResult(
                "UNKNOWN", "Failed", "Order input was missing.");
        }

        // Step 1: Reserve inventory.
        var inventory = await context.CallActivityAsync<InventoryResult>(
            nameof(OrderActivities.ReserveInventory), order);

        if (!inventory.Success)
        {
            return new OrderResult(
                order.OrderId, "Rejected", inventory.Message);
        }

        // Step 2: Wait for external approval OR a 5-minute timeout.
        var approvalTask =
            context.WaitForExternalEvent<bool>("OrderApproval");

        using var timeoutCts = new CancellationTokenSource();

        var deadline = context.CurrentUtcDateTime.AddMinutes(5);

        var timeoutTask = context.CreateTimer(
            deadline, timeoutCts.Token);

        var winner = await Task.WhenAny(approvalTask, timeoutTask);

        if (winner != approvalTask)
        {
            await context.CallActivityAsync(
                nameof(OrderActivities.ReleaseInventory), order);

            var timeoutResult = new OrderResult(
                order.OrderId,
                "ApprovalTimedOut",
                "Approval was not received within 5 minutes.");

            await context.CallActivityAsync(
                nameof(OrderActivities.SendOrderNotification),
                timeoutResult);

            return timeoutResult;
        }

        // Approval arrived first; cancel the outstanding timer.
        timeoutCts.Cancel();

        var approved = await approvalTask;

        if (!approved)
        {
            await context.CallActivityAsync(
                nameof(OrderActivities.ReleaseInventory), order);

            var rejectedResult = new OrderResult(
                order.OrderId,
                "ApprovalRejected",
                "Order approval was rejected.");

            await context.CallActivityAsync(
                nameof(OrderActivities.SendOrderNotification),
                rejectedResult);

            return rejectedResult;
        }

        // Step 3: Process payment with retries.
        var retryOptions = TaskOptions.FromRetryPolicy(
            new RetryPolicy(
                maxNumberOfAttempts: 3,
                firstRetryInterval: TimeSpan.FromSeconds(2),
                backoffCoefficient: 2.0));

        PaymentResult payment;

        try
        {
            payment = await context.CallActivityAsync<PaymentResult>(
                nameof(OrderActivities.ProcessPayment),
                order,
                retryOptions);
        }
        catch (TaskFailedException)
        {
            await context.CallActivityAsync(
                nameof(OrderActivities.ReleaseInventory), order);

            return new OrderResult(
                order.OrderId,
                "PaymentFailed",
                "Payment failed after retries. Inventory release was requested.");
        }

        if (!payment.Success)
        {
            await context.CallActivityAsync(
                nameof(OrderActivities.ReleaseInventory), order);

            return new OrderResult(
                order.OrderId,
                "PaymentFailed",
                payment.Message + " Inventory release was requested.");
        }

        // Step 4: Create shipment.
        var shipping = await context.CallActivityAsync<ShippingResult>(
            nameof(OrderActivities.CreateShipment), order);

        if (!shipping.Success)
        {
            return new OrderResult(
                order.OrderId, "ShippingFailed", shipping.Message);
        }

        // Step 5: Notify and return the final result.
        var result = new OrderResult(
            order.OrderId,
            "Completed",
            $"Payment: {payment.TransactionId}; Shipment: {shipping.ShipmentId}.");

        await context.CallActivityAsync(
            nameof(OrderActivities.SendOrderNotification), result);

        return result;
    }
}
