using MV.DomainLayer.Entities;

namespace MV.InfrastructureLayer.Interfaces;

/// <summary>
/// Interface cho Payment Repository
/// </summary>
public interface IPaymentRepository
{
    /// <summary>
    /// Tạo payment record
    /// </summary>
    Task<Payment> CreatePaymentAsync(Payment payment);

    /// <summary>
    /// Lấy payment theo order id
    /// </summary>
    Task<Payment?> GetByOrderIdAsync(int orderId);

    /// <summary>
    /// Lấy payment theo transaction id
    /// </summary>
    Task<Payment?> GetByTransactionIdAsync(string transactionId);

    /// <summary>
    /// Cập nhật payment
    /// </summary>
    Task UpdatePaymentAsync(Payment payment);
}
