namespace EzioLearning.Wasm.Common;

public abstract record RouteConstants
{
    public const string Home = "/";
    public const string Login = $"/{nameof(Login)}";
    public const string Course = $"/{nameof(Course)}";
    public const string About = $"/{nameof(About)}";
    public const string Contact = $"/{nameof(Contact)}";
    public const string Student = $"/{nameof(Student)}";
    public const string Register = $"/{nameof(Register)}";
    public const string ExternalLogin = $"/{nameof(ExternalLogin)}";
    public const string TermCondition = $"/{nameof(TermCondition)}";
    public const string PrivacyPolicy = $"/{nameof(PrivacyPolicy)}";
    public const string ForgotPassword = $"/{nameof(ForgotPassword)}";
    public const string ConfirmPassword = $"/{nameof(ConfirmPassword)}";

    public abstract record Account
    {
        public const string Home = $"/{nameof(Account)}/{nameof(RouteConstants.Home)}";
    }

    public abstract record Error
    {
        public const string UnAuthorized = $"/{nameof(Error)}/{nameof(UnAuthorized)}";
    }
}