using Microsoft.EntityFrameworkCore;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.DBContexts;
using MV.InfrastructureLayer.Interfaces;

namespace MV.InfrastructureLayer.Repositories;

public class StoreRepository : IStoreRepository
{
    private readonly ScamazonDbContext _context;

    public StoreRepository(ScamazonDbContext context)
    {
        _context = context;
    }

    public async Task<List<Store>> GetActiveStoresAsync()
    {
        return await _context.Stores
            .Where(s => s.IsActive == true)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Store?> GetByIdAsync(int id)
    {
        return await _context.Stores.FindAsync(id);
    }
}
