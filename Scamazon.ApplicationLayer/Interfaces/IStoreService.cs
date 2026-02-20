using MV.DomainLayer.DTO.ResponseModels;

namespace MV.ApplicationLayer.Interfaces;

public interface IStoreService
{
    Task<StoreInfoResponseDto> GetStoreInfoAsync();
}
