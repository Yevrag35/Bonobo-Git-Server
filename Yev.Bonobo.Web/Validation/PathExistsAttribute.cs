using System.ComponentModel.DataAnnotations;

namespace Yev.Bonobo.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class PathExistsAttribute : ValidationAttribute
{
    public bool IsFile { get; init; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        string? path = value as string;
        if (string.IsNullOrEmpty(path))
        {
            return ValidationResult.Success;
        }
        else if (this.IsFile && !File.Exists(path))
        {
            return new ValidationResult($"Unable to find file at path: '{path}'", validationContext.GetMemberNames());
        }
        else if (!Directory.Exists(path))
        {
            return new ValidationResult($"Unable to find directory at path: '{path}'", validationContext.GetMemberNames());
        }


        return ValidationResult.Success;
    }
}