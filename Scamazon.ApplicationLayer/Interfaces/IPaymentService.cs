using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.ApplicationLayer.Interfaces;

public interface IPaymentService
{
    Task<VNPayResponseDto> CreateVNPayUrlAsync(int userId, VNPayRequestDto request, string ipAddress, string baseUrl);
    Task<BaseResponseDto> ProcessVNPayCallbackAsync(Dictionary<string, string> vnpayData);
}
