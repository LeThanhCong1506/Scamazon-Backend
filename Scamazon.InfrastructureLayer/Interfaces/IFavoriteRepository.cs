using MV.DomainLayer.Entities;

namespace MV.InfrastructureLayer.Interfaces;

public interface IFavoriteRepository
{
    /// <summary>
    /// Lấy danh sách favorite của user (kèm thông tin product)
    /// </summary>
    Task<List<Favorite>> GetByUserIdAsync(int userId);

    /// <summary>
    /// Lấy danh sách product ID mà user đã favorite
    /// </summary>
    Task<List<int>> GetFavoriteProductIdsAsync(int userId);

    /// <summary>
    /// Tìm favorite theo userId và productId
    /// </summary>
    Task<Favorite?> FindAsync(int userId, int productId);

    /// <summary>
    /// Thêm favorite mới
    /// </summary>
    Task<Favorite> AddAsync(Favorite favorite);

    /// <summary>
    /// Xóa favorite
    /// </summary>
    Task DeleteAsync(Favorite favorite);
}
