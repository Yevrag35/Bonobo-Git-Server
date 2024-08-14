using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Yev.Bonobo.Database.Entities;

public sealed class RepoDefinition : IDbEntity<RepoDefinition>
{
    [Key]
    public Guid Id { get; set; }
    public string? DisplayName { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string? Description { get; set; }
    public bool AllowAnonymous { get; set; }
    public bool AuditPushUser { get; set; }
    public string? LinksRegex { get; set; }
    public string? LinksUrl { get; set; }
    public bool LinksUseGlobal { get; set; }

    public List<RepoToTag>? Tags { get; set; }

    static void IDbEntity<RepoDefinition>.AddToModel(ModelBuilder modelBuilder)
    {
        var repo = modelBuilder.Entity<RepoDefinition>().ToTable("Repositories");
        repo.HasKey(x => x.Id);
        repo.Property(x => x.Id).ValueGeneratedOnAdd();
        repo.Property(x => x.Name).IsRequired();
        repo.Property(x => x.Path).IsRequired();

        repo.HasMany(x => x.Tags).WithOne(x => x.Repo);

        repo.HasData(new RepoDefinition
        {
            Id = Guid.Parse("02f784d8-56a9-45ba-a380-cc7f08e29b37"),
            Name = "default",
            Path = "repos",
            Description = "The default repository",
            AllowAnonymous = false,
            AuditPushUser = true,
            DisplayName = "Default"
        });
    }
}