using Microsoft.EntityFrameworkCore;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.DBContexts;
using MV.InfrastructureLayer.Interfaces;

namespace MV.InfrastructureLayer.Repositories;

public class FavoriteRepository : IFavoriteRepository
{
    private readonly ScamazonDbContext _context;

    public FavoriteRepository(ScamazonDbContext context)
    {
        _context = context;
    }

    public async Task<List<Favorite>> GetByUserIdAsync(int userId)
    {
        return await _context.Favorites
            .Include(f => f.Product)
                .ThenInclude(p => p.ProductImages)
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<int>> GetFavoriteProductIdsAsync(int userId)
    {
        return await _context.Favorites
            .Where(f => f.UserId == userId)
            .Select(f => f.ProductId)
            .ToListAsync();
    }

    public async Task<Favorite?> FindAsync(int userId, int productId)
    {
        return await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
    }

    public async Task<Favorite> AddAsync(Favorite favorite)
    {
        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();
        return favorite;
    }

    public async Task DeleteAsync(Favorite favorite)
    {
        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();
    }
}
