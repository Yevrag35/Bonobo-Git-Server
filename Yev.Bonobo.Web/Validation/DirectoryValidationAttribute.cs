using System.ComponentModel.DataAnnotations;

namespace Yev.Bonobo.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class DirectoryValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        string? path = value as string;
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
        {
            return ValidationResult.Success;
        }

        try
        {
            DirectoryInfo dirInfo = new(path);
        }
        catch (Exception e)
        {
            return new ValidationResult(e.Message, validationContext.GetMemberNames());
        }
    }
}