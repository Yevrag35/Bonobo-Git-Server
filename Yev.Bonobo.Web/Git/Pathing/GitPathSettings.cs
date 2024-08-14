using System.ComponentModel.DataAnnotations;
using Yev.Bonobo.Validation;

namespace Yev.Bonobo.Git.Pathing;

public interface IGitPathSettings
{
    string? DefaultPath { get; }
    string DefaultPathBase { get; }
}

public sealed class GitPathSettings : IGitPathSettings
{
    [PathExists]
    [PathIsAccessible]
    public string? DefaultPath { get; set; }
    [Required]
    public string DefaultPathBase { get; set; } = "repos";

    public string GetFullPath(ReadOnlySpan<char> appPath)
    {
        if (!string.IsNullOrEmpty(this.DefaultPath))
        {
            return this.DefaultPath;
        }

        string path = Path.Join(appPath, this.DefaultPathBase);

        var dirInfo = Directory.CreateDirectory(path);
        return dirInfo.FullName;
    }
}