using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Yev.Bonobo.Database.Entities;

namespace Yev.Bonobo.Database;

public sealed class GitDbContext : DbContext
{
    public DbSet<RepoDefinition> Repos { get; }
    public DbSet<RepoToTag> RepoTags { get; }
    public DbSet<DataTag> Tags { get; }
    [NotNull]
    public DbSet<UserModel> Users { get; } = null!;

    public GitDbContext(DbContextOptions<GitDbContext> options) : base(options)
    {
        this.Users = this.Set<UserModel>();
        this.Repos = this.Set<RepoDefinition>();
        this.Tags = this.Set<DataTag>();
        this.RepoTags = this.Set<RepoToTag>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddToModel<UserModel>()
                    .AddToModel<RepoDefinition>()
                    .AddToModel<DataTag>()
                    .AddToModel<RepoToTag>();

        //_setter.StoreModel(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}