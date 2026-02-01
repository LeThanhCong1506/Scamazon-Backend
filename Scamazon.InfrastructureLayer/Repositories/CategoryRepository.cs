using Microsoft.EntityFrameworkCore;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.DBContexts;
using MV.InfrastructureLayer.Interfaces;

namespace MV.InfrastructureLayer.Repositories;

/// <summary>
/// Repository cho Category entity
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly ScamazonDbContext _context;

    public CategoryRepository(ScamazonDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy tất cả categories active
    /// </summary>
    public async Task<List<Category>> GetAllActiveAsync()
    {
        return await _context.Categories
            .Where(c => c.IsActive == true)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy categories theo parent_id
    /// </summary>
    public async Task<List<Category>> GetByParentIdAsync(int? parentId)
    {
        return await _context.Categories
            .Where(c => c.IsActive == true && c.ParentId == parentId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}