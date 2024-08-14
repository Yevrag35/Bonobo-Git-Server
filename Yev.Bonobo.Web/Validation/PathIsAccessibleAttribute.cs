using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace Yev.Bonobo.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class PathIsAccessibleAttribute : ValidationAttribute
{
    public bool IsFile { get; init; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ReadOnlySpan<char> path = value as string;
        
        if (this.IsFile)
        {
            path = Path.GetDirectoryName(path);
        }

        if (path.IsWhiteSpace())
        {
            return ValidationResult.Success;
        }

        string dirPath = path.ToString();
        if (!TryGetDirectoryInfo(validationContext, dirPath, out DirectoryInfo? dirInfo, out ValidationResult? result))
        {
            return result;
        }

        return ValidateDirectory(dirInfo, validationContext);
    }

    private static bool TryGetDirectoryInfo(ValidationContext ctx, string path, [NotNullWhen(true)] out DirectoryInfo? dirInfo, [NotNullWhen(false)] out ValidationResult? result)
    {
        try
        {
            dirInfo = new DirectoryInfo(path);
            result = null;
            return dirInfo.Exists;
        }
        catch (SecurityException e)
        {
            dirInfo = null;
            result = new ValidationResult(
                $"The directory path is inaccessible: {e.Message}",
                ctx.GetMemberNames());
            return false;
        }
    }

    private static ValidationResult? ValidateDirectory(DirectoryInfo dirInfo, ValidationContext ctx)
    {
        string randomName = Path.GetRandomFileName();
        string filePath = Path.Combine(dirInfo.FullName, randomName);
        Stream open = Stream.Null;

        try
        {
            FileInfo fi = new(filePath);
            open = fi.Create();
            open.Close();
            fi.Delete();
            return ValidationResult.Success;
        }
        catch (Exception e)
        {
            ValidationResult result = new(
                $"Unable to verify write access in directory '{dirInfo.FullName}': {e.Message}",
                ctx.GetMemberNames());

            return result;
        }
        finally
        {
            open.Dispose();
        }
    }
}