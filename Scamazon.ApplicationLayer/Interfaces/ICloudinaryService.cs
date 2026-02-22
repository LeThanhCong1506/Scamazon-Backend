namespace MV.ApplicationLayer.Interfaces;

/// <summary>
/// Service upload ảnh lên Cloudinary
/// </summary>
public interface ICloudinaryService
{
    /// <summary>
    /// Upload ảnh lên Cloudinary, trả về URL public
    /// </summary>
    /// <param name="fileStream">Stream của file ảnh</param>
    /// <param name="fileName">Tên file gốc</param>
    /// <param name="folder">Folder trên Cloudinary (optional)</param>
    Task<string> UploadImageAsync(Stream fileStream, string fileName, string? folder = null);
}
