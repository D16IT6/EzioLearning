using System.Text;

namespace EzioLearning.Api.Services;

public class FileService
{
    private static readonly string[] ImageExtensions = [".png", ".jpg", ".jpeg", ".bmp", ".ico"];

    public bool IsImageAccept(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return ImageExtensions.Contains(extension);
    }

    private string GenerateActuallyFilePath(string path)
    {
        var existCount = 0;
        var pathBuilder = new StringBuilder(path);
        while (File.Exists(pathBuilder.ToString()))
        {
            File.Delete(pathBuilder.ToString());
            existCount++;
            var prefix = Path.GetDirectoryName(path);
            var name = Path.GetFileNameWithoutExtension(path);
            var extension = Path.GetExtension(path);

            pathBuilder.Remove(0, pathBuilder.Length);

            pathBuilder.Append(prefix);
            pathBuilder.Append(Path.DirectorySeparatorChar);

            pathBuilder.Append(name);
            pathBuilder.Append($" ({existCount})");
            pathBuilder.Append(extension);
        }

        return pathBuilder.ToString();
    }

    public async Task<string> SaveFile(IFormFile file, string folderPath, string outputFileNameWithoutExtension)
    {
        var tempFilePath = Path.Combine(folderPath, outputFileNameWithoutExtension + Path.GetExtension(file.FileName));

        //var actuallyFilePath = GenerateActuallyFilePath(Path.Combine(Environment.CurrentDirectory, "wwwroot", tempFilePath));

        var oldFilePath = Path.Combine(Environment.CurrentDirectory, "wwwroot", tempFilePath);
        if (File.Exists(oldFilePath)) File.Delete(oldFilePath);

        await using var fileStream = new FileStream(oldFilePath, FileMode.CreateNew);

        await file.CopyToAsync(fileStream);

        var result = Path.Combine(folderPath, Path.GetFileName(oldFilePath));

        return result;
    }
}