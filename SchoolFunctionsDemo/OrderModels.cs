namespace SchoolFunctionsDemo.SchoolFunctionsDemo;

public record OrderRequest(
    string OrderId,
    string ProductId,
    int Quantity,
    decimal Amount);

public record InventoryResult(
    bool Success,
    string Message);

public record PaymentResult(
    bool Success,
    string TransactionId,
    string Message);

public record ShippingResult(
    bool Success,
    string ShipmentId,
    string Message);

public record OrderResult(
    string OrderId,
    string Status,
    string Message);

public record OrderApprovalRequest(
    string InstanceId,
    bool Approved);