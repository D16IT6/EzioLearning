namespace EzioLearning.Share.Validators.Common;

public static class ValidatorDateCommon
{
    public static bool BeValidDate(this DateTime? dateOfBirth)
    {
        return dateOfBirth <= DateTime.UtcNow.AddYears(-10) &&
               dateOfBirth >= DateTime.UtcNow.AddYears(-100);
    }
}