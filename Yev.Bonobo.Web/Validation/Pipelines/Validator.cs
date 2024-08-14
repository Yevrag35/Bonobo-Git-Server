namespace Yev.Bonobo.Validation.Pipelines;

/// <summary>
/// An <see langword="abstract"/>, base class implementation providing the base 
/// functionality of validating properties/logic on a given database entity.
/// </summary>
public abstract class Validator
{
    /// <summary>
    /// An <see langword="async"/> method that returns whether this implementation should
    /// perform validation on the given entity.
    /// </summary>
    /// <param name="instance">The entity that is to be validated.</param>
    /// <param name="context">
    ///     A contextual object with properties and methods that
    ///     can be used by this validator implementation.
    /// </param>
    /// <param name="token">
    ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask"/> that will eventually return either <see langword="true"/> if 
    ///     this validator should execute on <paramref name="instance"/>; otherwise, 
    ///     <see langword="false"/>, meaning this validator should skip <paramref name="instance"/>.
    /// </returns>
    public virtual ValueTask<bool> ShouldValidateAsync(object instance, DbValidationContext context, CancellationToken token)
    {
        return ValueTask.FromResult(true);
    }

    /// <inheritdoc cref="ShouldValidateAsync(object, DbValidationContext, CancellationToken)"
    ///     path="/*[not(self::summary) and not(self::returns)]"/>
    /// <summary>
    ///     An <see langword="async"/> method that performs one or more validation tests on the specified
    ///     object with any validation failures being written to <paramref name="context"/> using
    ///     one of the <c>AddError</c> overloads.
    /// </summary>
    public abstract Task ValidateAsync(object instance, DbValidationContext context, CancellationToken token);
}

/// <summary>
/// An <see langword="abstract"/>, <see cref="Validator"/> class providing the
/// functionality of validating properties/logic on a given database entity of type 
/// <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the entity being validated.</typeparam>
public abstract class Validator<T> : Validator where T : class
{
    /// <inheritdoc/>
    public sealed override ValueTask<bool> ShouldValidateAsync(object instance, DbValidationContext context, CancellationToken token)
    {
        if (instance is not T entity)
        {
            return ValueTask.FromResult(false);
        }

        return this.ShouldValidateAsync(entity: entity, context, token);
    }

    /// <inheritdoc cref="ShouldValidateAsync(object, DbValidationContext, CancellationToken)"
    ///     path="/*[not(self::returns)]"/>
    /// <param name="entity"><inheritdoc cref="ShouldValidateAsync(object, DbValidationContext, CancellationToken)"
    ///     path="/param[1]"/>
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask"/> that will eventually return either <see langword="true"/> if 
    ///     this validator should execute on <paramref name="entity"/>; otherwise, 
    ///     <see langword="false"/>, meaning this validator should skip <paramref name="entity"/>.
    /// </returns>
    public abstract ValueTask<bool> ShouldValidateAsync(T entity, DbValidationContext context, CancellationToken token);

    /// <inheritdoc/>
    public sealed override Task ValidateAsync(object instance, DbValidationContext context, CancellationToken token)
    {
        if (instance is not T entity)
        {
            return Task.CompletedTask;
        }

        return this.ValidateAsync(entity: entity, context, token);
    }

    /// <inheritdoc cref="ValidateAsync(object, DbValidationContext, CancellationToken)"/>
    public abstract Task ValidateAsync(T entity, DbValidationContext context, CancellationToken token);
}
