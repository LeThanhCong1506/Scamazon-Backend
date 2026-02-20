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

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> SlugExistsAsync(string slug)
    {
        return await _context.Categories.AnyAsync(c => c.Slug == slug);
    }

    public async Task<Category> CreateAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(int id)
    {
        await _context.Categories
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsActive, false));
    }
}