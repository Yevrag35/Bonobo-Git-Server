using AttributeDI.Attributes;
using Microsoft.Extensions.Options;
using System.ComponentModel;

namespace Yev.Bonobo.Git.Pathing;

public interface IPathService
{
    ReadOnlySpan<char> ResolveRepoPath(ReadOnlySpan<char> absoluteOrRelativePath);
}

[DynamicServiceRegistration]
internal sealed class PathService : IPathService
{
    private readonly string _repoPath;

    public PathService(IOptions<GitPathSettings> settings)
    {
        _repoPath = InitializePath(settings.Value);
    }

    public ReadOnlySpan<char> ResolveRepoPath(ReadOnlySpan<char> absoluteOrRelativePath)
    {
        if (Path.IsPathFullyQualified(absoluteOrRelativePath))
        {
            return absoluteOrRelativePath;
        }

        return Path.Join(_repoPath, absoluteOrRelativePath);
    }

    private static string InitializePath(GitPathSettings settings)
    {
        string fullRepoPath = settings.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
        settings.DefaultPath = fullRepoPath;

        return fullRepoPath;
    }
    [DynamicServiceRegistrationMethod]
    [EditorBrowsable(EditorBrowsableState.Never)]
    private static void Register(IServiceCollection services, IConfiguration configuration)
    {
        var options = services.AddOptions<GitPathSettings>();
        options.Bind(configuration.GetRequiredSection("Git").GetRequiredSection("Defaults"))
               .ValidateDataAnnotations()
               .ValidateOnStart();

        services.AddSingleton<IPathService, PathService>();
    }
}