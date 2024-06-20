using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace EzioLearning.Wasm.Utils.Extensions
{
    static class BrowserFileExtensions
    {
        private static double MaxBlobSizeMb { get; set; } = 2 * 1024;//3GB
        private static double MaxBlobSizePerFileMb { get; } = MaxBlobSizeMb / 10;
        public static string ShowFormatFileSize(this IBrowserFile file)
        {
            string[] suffixes = ["B", "KB", "MB", "GB", "TB", "PB", "EB"];
            var counter = 0;
            decimal number = file.Size;

            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }

            return $"{number:n1} {suffixes[counter]}";
        }
        public static string ShowFormatFileSize(this long fileSize)
        {
            string[] suffixes = ["B", "KB", "MB", "GB", "TB", "PB", "EB"];
            var counter = 0;

            decimal number = fileSize;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }

            return $"{number:n1} {suffixes[counter]}";
        }

        public static string ShowShortedFileName(this IBrowserFile file, int prefixLength = 15, int prefixExtensionLength = 3)
        {
            var length = Path.GetFileNameWithoutExtension(file.Name).Length;

            return $"{file.Name[..prefixLength]}...{file.Name[(length - prefixExtensionLength)..]}";
        }

        public static async Task<string> GetBlobStream(this IBrowserFile file, IJSRuntime jsRuntime)
        {
            var fileSize = file.Size;
            var megaBytes = fileSize / 1024.0 / 1024.0;

            if (MaxBlobSizeMb - megaBytes < 0 || megaBytes > MaxBlobSizePerFileMb)
            {
                await jsRuntime.InvokeVoidAsync("alert",
                    "Buffer limit exceeded, preview file does not work");
                return string.Empty;
            }

            var buffer = new byte[fileSize];
            var bytes = await file.OpenReadStream(fileSize).ReadAsync(buffer);
            if (bytes != fileSize) return string.Empty;

            MaxBlobSizeMb -= megaBytes;
            return await jsRuntime.InvokeAsync<string>($"blobService.{nameof(GetBlobStream)}", buffer, file.ContentType);
        }
    }
}
