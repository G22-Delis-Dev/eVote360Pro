using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace eVote360Pro.Web.Helpers;

public static class SubidaArchivo
{
    public static string? Subir(IFormFile? file, string folderName, bool isEditMode = false, string? imagePath = "")
    {
        if (isEditMode && file == null)
        {
            return imagePath;
        }

        if (file == null)
        {
            return string.Empty;
        }

        string extension = Path.GetExtension(file.FileName).ToLower();
        if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
        {
            throw new eVote360Pro.Domain.Exceptions.ValidacionException("El archivo seleccionado no tiene un formato de imagen válido.");
        }

        string basePath = $"img/{folderName}";
        string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/{basePath}");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        Guid guid = Guid.NewGuid();
        FileInfo fileInfo = new(file.FileName);
        string fileName = guid + fileInfo.Extension;

        string fullFilePath = Path.Combine(path, fileName);

        using (var stream = new FileStream(fullFilePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        if (isEditMode && !string.IsNullOrWhiteSpace(imagePath))
        {
            string[] oldImagePart = imagePath.Split("/");
            string oldFileName = oldImagePart[^1]; // Obtiene el nombre del archivo antiguo
            string completeOldPath = Path.Combine(path, oldFileName);

            if (File.Exists(completeOldPath))
            {
                File.Delete(completeOldPath);
            }
        }

        return $"/{basePath}/{fileName}";
    }
}
