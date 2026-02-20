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

    public async Task<Brand?> GetByIdAsync(int id)
    {
        return await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<bool> SlugExistsAsync(string slug)
    {
        return await _context.Brands.AnyAsync(b => b.Slug == slug);
    }

    public async Task<Brand> CreateAsync(Brand brand)
    {
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();
        return brand;
    }

    public async Task<Brand> UpdateAsync(Brand brand)
    {
        _context.Brands.Update(brand);
        await _context.SaveChangesAsync();
        return brand;
    }

    public async Task DeleteAsync(int id)
    {
        await _context.Brands
            .Where(b => b.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.IsActive, false));
    }
}