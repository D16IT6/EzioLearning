using System.Text;

namespace EzioLearning.Api.Services
{
	public class FileService
	{
		private static readonly string[] ImageExtensions = [".png", ".jpg", ".jpeg", ".bmp", ".ico"];

		public bool IsImageAccept(string fileName)
		{
			var extension = Path.GetExtension(fileName);
			return ImageExtensions.Contains(extension);
		}

		public string GenerateActuallyFilePath(string path)
		{
			var existCount = 0;
			var pathBuilder = new StringBuilder(path);
			while (File.Exists(pathBuilder.ToString()))
			{
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
	}
}
