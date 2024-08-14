using AttributeDI.Attributes;
using Yev.Bonobo.Database;
using Yev.Bonobo.Database.Entities;
using Yev.Bonobo.Git.Pathing;

namespace Yev.Bonobo.Git;

public interface IRepoService
{
    //void CreateRepo(string name, string path, string description);
    //void DeleteRepo(string name);
    //void RenameRepo(string oldName, string newName);
}

[ServiceRegistration(typeof(IRepoService))]
internal sealed class RepoService : IRepoService
{
    private readonly IPathService _pathSvc;
    private readonly IScopedProviderFactory _scopeFac;

    public RepoService(IScopedProviderFactory factory, IPathService pathSvc)
    {
        _pathSvc = pathSvc;
        _scopeFac = factory;
    }

    public async Task<RepoDefinition> CreateAsync<T>(DbExpressionState<T> state, IServiceProvider? provider, Action<T, RepoDefinition> setProperties)
    {
        await using var scope = _scopeFac.CreateOptionalAsyncScope(provider);
        var dbCtx = scope.ServiceProvider.GetRequiredService<GitDbContext>();

        RepoDefinition definition = new();
        setProperties(state, definition);

        
    }
    public GitRepository Initialize(string path, RepoDefinition definition)
    {
        path = _pathSvc.ResolveRepoPath(path).ToString();
        var dirInfo = Directory.CreateDirectory(path);

        return new GitRepository(definition);
    }
}