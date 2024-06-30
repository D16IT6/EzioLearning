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

    public const string Checkout = $"/{nameof(Checkout)}";


    public abstract record AccountRoute
    {
        public const string Home = $"Account/{nameof(Home)}";
        public const string Report = $"Account/{nameof(Report)}";
        public const string Security = $"Account/{nameof(Security)}";
        public const string ConfirmChangeEmail = $"Account/{nameof(ConfirmChangeEmail)}";
        public const string ConfirmChangePassword = $"Account/{nameof(ConfirmChangePassword)}";
        public const string Delete = $"Account/{nameof(Delete)}";
        public const string PurchasedCourse = $"Account/{nameof(PurchasedCourse)}";


        public const string CourseIndex = "/Account/Course/Index";

        public const string CourseCreate = "Account/Course/Create";

        public const string CourseUpdate = "Account/Course/Update/{CourseId:guid}";
        public const string CourseUpdateNoParam = "Account/Course/Update";
    }

    public record CourseRoute
    {
        public const string CourseDetail = "/Course/Detail/{CourseId:guid}";
        public const string CourseDetailNoParam = "/Course/Detail";
    }

    public abstract record ErrorRoute
    {
        public const string UnAuthorized = $"/ErrorRoute/{nameof(UnAuthorized)}";
    }
}