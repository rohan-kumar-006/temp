namespace InventoryManagement.API.Services.Interfaces;

public interface IFileService
{
    Task<String?> SaveImageAsync(IFormFile? file);
    void DeleteImage(string? imagePath);
}