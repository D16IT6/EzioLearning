namespace EzioLearning.Wasm.Utils.Common;

public abstract record RouteConstants
{
    public const string Index = "/";
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
    public const string Logout = $"/{nameof(Logout)}";
    public const string DevLater = $"/{nameof(DevLater)}";

    public abstract record AccountRoute
    {
        public const string Home = $"Account/{nameof(Home)}";
        public const string Security = $"Account/{nameof(Security)}";
        public const string ConfirmChangeEmail = $"Account/{nameof(ConfirmChangeEmail)}";
        public const string ConfirmChangePassword = $"Account/{nameof(ConfirmChangePassword)}";
        public const string Delete = $"Account/{nameof(Delete)}";

        public const string CourseCreate = "Account/Course/Create";
    }

    public  record CourseRoute
    {
        //public const string Index = $"/Course";
    }

    public abstract record ErrorRoute
    {
        public const string UnAuthorized = $"/ErrorRoute/{nameof(UnAuthorized)}";
    }
}