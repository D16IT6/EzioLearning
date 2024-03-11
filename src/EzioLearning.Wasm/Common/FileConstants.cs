namespace EzioLearning.Wasm.Common
{
    public class FileConstants
    {
        public static string[] AcceptTypes { get; } = [".png", ".bmp", ".jpg", ".jpeg"];
        public const long UploadLimit = 10 * 1024 * 1024;
        public const string FileTooLargeMessage = "File quá lớn, không được phép!";
        public const string FileNowAllowExtensionMessage = "Không cho phép file định dạng này!";

    }
}
