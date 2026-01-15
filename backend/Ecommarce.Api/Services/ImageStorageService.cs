using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Ecommarce.Api.Services;

public interface IImageStorageService
{
    Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default);
    string? NormalizeImageName(string? value);
    string? BuildPublicUrl(HttpRequest request, string? value);
}

public sealed class ImageStorageService : IImageStorageService
{
    private const string UploadsFolder = "uploads";
    private readonly IWebHostEnvironment _environment;

    public ImageStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(file.FileName);
        var safeExtension = string.IsNullOrWhiteSpace(extension) ? string.Empty : extension;
        var fileName = $"{Guid.NewGuid():N}{safeExtension}";
        var root = ResolveWebRootPath();
        var uploadsPath = Path.Combine(root, UploadsFolder);

        Directory.CreateDirectory(uploadsPath);

        var destination = Path.Combine(uploadsPath, fileName);
        await using var stream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None);
        await file.CopyToAsync(stream, cancellationToken);

        return fileName;
    }

    public string? NormalizeImageName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var trimmed = value.Trim();
        if (trimmed.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed;
        }

        if (Uri.TryCreate(trimmed, UriKind.Absolute, out var uri))
        {
            if (uri.AbsolutePath.Contains($"/{UploadsFolder}/", StringComparison.OrdinalIgnoreCase))
            {
                return Path.GetFileName(uri.LocalPath);
            }

            return trimmed;
        }

        if (trimmed.StartsWith($"/{UploadsFolder}/", StringComparison.OrdinalIgnoreCase) ||
            trimmed.StartsWith($"{UploadsFolder}/", StringComparison.OrdinalIgnoreCase))
        {
            return Path.GetFileName(trimmed);
        }

        return Path.GetFileName(trimmed);
    }

    public string? BuildPublicUrl(HttpRequest request, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        if (value.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            return value;
        }

        if (Uri.TryCreate(value, UriKind.Absolute, out _))
        {
            return value;
        }

        var baseUrl = $"{request.Scheme}://{request.Host}";
        var trimmed = value.TrimStart('/');
        if (trimmed.StartsWith($"{UploadsFolder}/", StringComparison.OrdinalIgnoreCase))
        {
            return $"{baseUrl}/{trimmed}";
        }

        return $"{baseUrl}/{UploadsFolder}/{trimmed}";
    }

    private string ResolveWebRootPath()
    {
        if (!string.IsNullOrWhiteSpace(_environment.WebRootPath))
        {
            return _environment.WebRootPath;
        }

        return Path.Combine(_environment.ContentRootPath, "wwwroot");
    }
}
