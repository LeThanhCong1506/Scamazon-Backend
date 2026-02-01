using Microsoft.EntityFrameworkCore;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.DBContexts;
using MV.InfrastructureLayer.Interfaces;

namespace MV.InfrastructureLayer.Repositories;

/// <summary>
/// Repository cho Brand entity
/// </summary>
public class BrandRepository : IBrandRepository
{
    private readonly ScamazonDbContext _context;

    public BrandRepository(ScamazonDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy tất cả brands active
    /// </summary>
    public async Task<List<Brand>> GetAllActiveAsync()
    {
        return await _context.Brands
            .Where(b => b.IsActive == true)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }
}