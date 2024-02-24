using System.ComponentModel.DataAnnotations;

namespace EzioLearning.Wasm.Attributes
{
    public class RequireTrueAttribute : ValidationAttribute
    {
        public RequireTrueAttribute()
        {
            ErrorMessage = "{0} là bắt buộc";
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return false;
            }

            return (bool)value == true;
        }
    }
}
