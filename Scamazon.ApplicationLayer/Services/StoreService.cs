using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.ResponseModels;
using MV.InfrastructureLayer.Interfaces;

namespace MV.ApplicationLayer.Services;

public class StoreService : IStoreService
{
    private readonly IStoreRepository _storeRepository;

    public StoreService(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    public async Task<StoreInfoResponseDto> GetStoreInfoAsync()
    {
        var stores = await _storeRepository.GetActiveStoresAsync();

        var data = stores.Select(s => new StoreDataDto
        {
            Id = s.Id,
            Name = s.Name,
            Address = s.Address,
            Phone = s.Phone,
            Latitude = s.Latitude,
            Longitude = s.Longitude,
            OpeningHours = s.OpeningHours,
            IsActive = s.IsActive
        }).ToList();

        return new StoreInfoResponseDto
        {
            Success = true,
            Message = "Lấy thông tin cửa hàng thành công",
            Data = data
        };
    }
}
