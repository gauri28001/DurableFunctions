
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SchoolFunctionsDemo.SchoolFunctionsDemo;

namespace SchoolFunctionsDemo.SchoolFunctionsDemo;

public class OrderActivities
{
    private readonly ILogger<OrderActivities> _logger;

    public OrderActivities(ILogger<OrderActivities> logger)
    {
        _logger = logger;
    }

    [Function(nameof(ReserveInventory))]
    public InventoryResult ReserveInventory(
        [ActivityTrigger] OrderRequest order)
    {
        _logger.LogInformation(
            "Checking inventory for {ProductId}, quantity {Quantity}",
            order.ProductId,
            order.Quantity);

        // Simulate inventory validation.
        if (order.Quantity > 10)
        {
            return new InventoryResult(
                false,
                "Insufficient inventory.");
        }

        _logger.LogInformation(
            "Inventory reserved for order {OrderId}",
            order.OrderId);

        return new InventoryResult(
            true,
            "Inventory reserved successfully.");
    }

    [Function(nameof(ProcessPayment))]
    public PaymentResult ProcessPayment(
        [ActivityTrigger] OrderRequest order)
    {
        _logger.LogInformation(
            "Processing payment for order {OrderId}",
            order.OrderId);

        if (order.ProductId == "PAYMENT-FAIL")
        {
            return new PaymentResult(
                false,
                string.Empty,
                "Payment was declined.");
        }

        if (order.ProductId == "PAYMENT-ERROR")
        {
            throw new InvalidOperationException(
                "Simulated temporary payment service error.");
        }

        return new PaymentResult(
            true,
            $"TXN-{order.OrderId}",
            "Payment completed successfully.");
    }

    [Function(nameof(CreateShipment))]
    public ShippingResult CreateShipment(
        [ActivityTrigger] OrderRequest order)
    {
        _logger.LogInformation(
            "Creating shipment for order {OrderId}",
            order.OrderId);

        return new ShippingResult(
            true,
            $"SHIP-{order.OrderId}",
            "Shipment created successfully.");
    }

    [Function(nameof(SendOrderNotification))]
    public void SendOrderNotification(
        [ActivityTrigger] OrderResult result)
    {
        _logger.LogInformation(
            "Order notification: {OrderId}, status {Status}",
            result.OrderId,
            result.Status);
    }

    [Function(nameof(ReleaseInventory))]
    public void ReleaseInventory(
        [ActivityTrigger] OrderRequest order)
    {
        _logger.LogWarning(
            "Compensating order {OrderId}: releasing {Quantity} units of {ProductId}",
            order.OrderId,
            order.Quantity,
            order.ProductId);
    }

}
