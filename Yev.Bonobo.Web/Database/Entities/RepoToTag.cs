using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yev.Bonobo.Database.Entities;

public sealed class RepoToTag : IDbEntity<RepoToTag>
{
    [ForeignKey("FK_Repo_Tag")]
    public required Guid RepoId { get; set; }
    public RepoDefinition? Repo { get; set; }
    [ForeignKey("FK_Tag_Repo")]
    public required int DataTagId { get; set; }
    public DataTag? Tag { get; set; }

    static void IDbEntity<RepoToTag>.AddToModel(ModelBuilder modelBuilder)
    {
        var en = modelBuilder.Entity<RepoToTag>();
        en.HasKey(x => new { x.RepoId, x.DataTagId });

        en.HasOne(x => x.Repo).WithMany(x => x.Tags).HasForeignKey(x => x.RepoId)
            .OnDelete(DeleteBehavior.Cascade);

        en.HasOne(x => x.Tag).WithMany(x => x.Repos).HasForeignKey(x => x.DataTagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}