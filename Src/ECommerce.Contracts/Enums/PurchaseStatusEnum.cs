namespace ECommerce.Contracts.Enums;

public enum PurchaseStatusEnum : short
{
    WaitingPayment = 100,
    PaymentApproved = 200,
    Shipping = 201,
    Delivered = 202,
    Rejected = 300,
    Cancelled = 400
}