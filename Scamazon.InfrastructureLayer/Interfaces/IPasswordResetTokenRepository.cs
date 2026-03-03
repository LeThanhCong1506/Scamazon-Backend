using MV.DomainLayer.Entities;

namespace MV.InfrastructureLayer.Interfaces;

public interface IPasswordResetTokenRepository
{
    /// <summary>
    /// Lưu OTP mới vào database
    /// </summary>
    Task SaveOtpAsync(PasswordResetToken token);

    /// <summary>
    /// Lấy OTP hợp lệ (chưa dùng và chưa hết hạn)
    /// </summary>
    Task<PasswordResetToken?> GetValidOtpAsync(string email, string otp);

    /// <summary>
    /// Vô hiệu hóa tất cả OTP cũ của email (trước khi tạo mới)
    /// </summary>
    Task InvalidateOldOtpsAsync(string email);

    /// <summary>
    /// Đánh dấu OTP đã sử dụng
    /// </summary>
    Task MarkAsUsedAsync(PasswordResetToken token);
}
