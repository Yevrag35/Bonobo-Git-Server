using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Yev.Bonobo.Database.Entities;

public sealed class DataTag : IDbEntity<DataTag>
{
    [Key]
    public int Id { get; set; }
    public required string Label { get; set; }

    public List<RepoToTag>? Repos { get; set; }

    static void IDbEntity<DataTag>.AddToModel(ModelBuilder modelBuilder)
    {
        var en = modelBuilder.Entity<DataTag>().ToTable("DataTags");
        en.HasKey(x => x.Id);
        en.Property(x => x.Label).IsRequired();
        en.HasMany(x => x.Repos).WithOne(x => x.Tag);

        en.HasData(new DataTag
        {
            Id = 1,
            Label = "default"
        });
    }
}