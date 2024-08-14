using System.ComponentModel.DataAnnotations;

namespace Yev.Bonobo.Validation;

public static class ValidationContextExtensions
{
    public static string[] GetMemberNames(this ValidationContext context)
    {
        if (!string.IsNullOrEmpty(context.MemberName))
        {
            return [context.MemberName];
        }
        else if (!string.IsNullOrEmpty(context.DisplayName))
        {
            return [context.DisplayName];
        }
        else
        {
            return [];
        }
    }
}