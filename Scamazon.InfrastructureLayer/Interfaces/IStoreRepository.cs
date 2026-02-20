using MV.DomainLayer.Entities;

namespace MV.InfrastructureLayer.Interfaces;

public interface IStoreRepository
{
    Task<List<Store>> GetActiveStoresAsync();
    Task<Store?> GetByIdAsync(int id);
}
