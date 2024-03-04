namespace EzioLearning.Share.Validators.Common;

public static class ValidatorDateCommon
{
    public static bool BeValidDate(this DateOnly dateOfBirth)
    {
        return dateOfBirth <= DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-10) &&
               dateOfBirth >= DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-100);
    }
}