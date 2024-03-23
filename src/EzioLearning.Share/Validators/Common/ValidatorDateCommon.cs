namespace EzioLearning.Share.Validators.Common;

public static class ValidatorDateCommon
{
    public static bool BeValidDate(this DateTime? dateOfBirth,int minYear,int maxYear)
    {
        return dateOfBirth <= DateTime.UtcNow.AddYears(-minYear) &&
               dateOfBirth >= DateTime.UtcNow.AddYears(-maxYear);
    }
}