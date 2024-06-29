using EzioLearning.Share.Validators.Common;

namespace EzioLearning.Api.Services;

public class FileService
{
    public bool IsImageAccept(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return FileConstants.ImageAcceptTypes.Contains(extension);
    }

    public async Task<string> SaveFile(IFormFile file, string folderPath, string outputFileNameWithoutExtension)
    {

        var tempFilePath = Path.Combine(folderPath, outputFileNameWithoutExtension + Path.GetExtension(file.FileName));

        //var actuallyFilePath = GenerateActuallyFilePath(Path.Combine(Environment.CurrentDirectory, "wwwroot", tempFilePath));

        var actuallyFilePath = Path.Combine(Environment.CurrentDirectory, "wwwroot", tempFilePath);
        if (File.Exists(actuallyFilePath)) File.Delete(actuallyFilePath);

        var newDirectory = Path.GetDirectoryName(actuallyFilePath);
        if (!Directory.Exists(newDirectory) && !string.IsNullOrWhiteSpace(newDirectory))
        {
            Directory.CreateDirectory(newDirectory);
        }
        await using var fileStream = new FileStream(actuallyFilePath, FileMode.CreateNew);

        
        await file.CopyToAsync(fileStream);

        var result = Path.Combine(folderPath, Path.GetFileName(actuallyFilePath)).Replace(Path.DirectorySeparatorChar.ToString(),"/").Replace("\\","/");

        return result;
    }
}