using LibGit2Sharp;
using Yev.Bonobo.Database.Entities;

namespace Yev.Bonobo.Git;

public sealed class GitRepository : ViolatileGitObject
{
    private readonly Repository _repo;
    protected override IDisposable Violatile => _repo;

    public GitRepository(RepoDefinition definition)
    {
        _repo = new(definition.Path);
    }
}