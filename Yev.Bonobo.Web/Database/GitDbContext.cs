using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Yev.Bonobo.Database.Entities;

namespace Yev.Bonobo.Database;

public sealed class GitDbContext : DbContext
{
    [NotNull]
    public DbSet<UserModel> Users { get; } = null!;

    //private readonly IDbContextModelCacher _setter;
    public GitDbContext(DbContextOptions<GitDbContext> options) : base(options)
    {
        //_setter = setter;
        this.Users = this.Set<UserModel>();
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    //_setter.UseModel(optionsBuilder);
    //    base.OnConfiguring(optionsBuilder);
    //}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<UserModel>().ToTable("Users");
            
        user.HasKey(x => x.Id);

        user.HasData(new UserModel
        {
            UserName = "admin",
            EntraIdObjectId = Guid.Parse("da049019-bbfc-480f-9ab6-a16c4829db20"),
            Id = 1,
        });

        //_setter.StoreModel(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}