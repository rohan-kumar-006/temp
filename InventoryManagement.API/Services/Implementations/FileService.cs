using System.Diagnostics;
using InventoryManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace InventoryManagement.API.Services.Implementations;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;
    public FileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<String?> SaveImageAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }
        // Console.WriteLine($"WebRootPath = {_environment.WebRootPath}");
        // Console.WriteLine($"ContentRootPath = {_environment.ContentRootPath}");
        var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "products");

        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        var extension = Path.GetExtension(file.FileName);

        var fileName = $"{Guid.NewGuid()}{extension}";

        var filePath = Path.Combine(uploadFolder, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);

        await file.CopyToAsync(stream);

        return $"/uploads/products/{fileName}";
    }

    public void DeleteImage(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return;
        }
        var filePath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
    }
}