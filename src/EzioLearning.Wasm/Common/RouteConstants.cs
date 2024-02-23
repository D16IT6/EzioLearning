namespace EzioLearning.Wasm.Common
{
    public record RouteConstants
    {
        public const string Index = "/";
        public const string Login = $"/{nameof(Login)}";
        public const string Course = $"/{nameof(Course)}";
        public const string About = $"/{nameof(About)}";
        public const string Contact = $"/{nameof(Contact)}";
        public const string Student = $"/{nameof(Student)}";
        public const string Signup = $"/{nameof(Signup)}";
    }
}
