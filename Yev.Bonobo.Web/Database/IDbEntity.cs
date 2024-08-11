using Microsoft.EntityFrameworkCore;

namespace Yev.Bonobo.Database;

public interface IDbEntity<T> where T : class, IDbEntity<T>
{
    static abstract void AddToModel(ModelBuilder modelBuilder);
}

public static class DbModelBuildingExtensions
{
    public static ModelBuilder AddToModel<T>(this ModelBuilder builder) where T : class, IDbEntity<T>
    {
        T.AddToModel(builder);
        return builder;
    }
}