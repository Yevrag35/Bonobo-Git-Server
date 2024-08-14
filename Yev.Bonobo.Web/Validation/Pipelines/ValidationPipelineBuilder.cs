using System.Collections;
using Yev.Bonobo.Database;

namespace Yev.Bonobo.Validation.Pipelines;

public sealed class ValidationPipelineBuilder : IEnumerable<KeyValuePair<Type, PipelineRegistrator>>
{
    private readonly Dictionary<Type, PipelineRegistrator> _registrations;

    public ValidationPipelineBuilder()
    {
        _registrations = new(10);
    }

    public IEnumerator<KeyValuePair<Type, PipelineRegistrator>> GetEnumerator()
    {
        return _registrations.GetEnumerator();
    }

    public PipelineRegistrator<TEntity> Register<TEntity>() where TEntity : class, IDbEntity<TEntity>
    {
        PipelineRegistrator<TEntity> registrator = new();
        _registrations.Add(registrator.EntityType, registrator);

        return registrator;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
