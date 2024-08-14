using Yev.Bonobo.Database;

namespace Yev.Bonobo.Validation.Pipelines;

public abstract class PipelineRegistrator
{
    protected List<Type> ValidatorTypes { get; }

    internal ImmutableArray<Type> GetSteps() => this.ValidatorTypes.ToImmutableArray();

    protected PipelineRegistrator()
    {
        this.ValidatorTypes = new(2);
    }
}
public sealed class PipelineRegistrator<TEntity> : PipelineRegistrator where TEntity : class, IDbEntity<TEntity>
{
    public Type EntityType { get; }

    public PipelineRegistrator()
        : base()
    {
        this.EntityType = typeof(TEntity);
    }

    public PipelineRegistrator<TEntity> AddStep<TValidator>() where TValidator : Validator<TEntity>
    {
        this.ValidatorTypes.Add(typeof(TValidator));
        return this;
    }
    //[Conditional("DEBUG")]
    //private void ValidateTheValidator(Type validatorType)
    //{

    //}
}