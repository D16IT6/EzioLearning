namespace EzioLearning.Share.Utils;

public abstract record RoleConstants
{
    public static string Admin = $"{nameof(Admin)}";
    public static string Teacher = $"{nameof(Teacher)}";
    public static string User = $"{nameof(User)}";
}