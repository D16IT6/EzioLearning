namespace EzioLearning.Share.Validators.Common
{
    public static class FileConstants
    {
        public static string[] ImageAcceptTypes { get; } = [".png", ".bmp", ".jpg", ".jpeg"];
        public static string[] VideoAcceptTypes { get; } = [".mp4", ".mkv",".m4v"];
        public static string[] DocumentAcceptTypes { get; } = [".doc", ".docx",".txt",".xls",".xlsx",".csv",".pdf"];
        public const long ImageUploadLimit = 10 * 1024 * 1024;
        public const long  VideoUploadLimit = 1024 * 1024 * 1024;
        public const long  DocumentUploadLimit = 50 * 1024 * 1024;
        public const string FileTooLargeMessage = "File quá lớn, không được phép!";
        public const string FileNowAllowExtensionMessage = "Không cho phép file định dạng này!";

    }
}
