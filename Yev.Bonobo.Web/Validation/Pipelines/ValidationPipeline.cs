namespace Yev.Bonobo.Validation.Pipelines;

public interface IValidationPipeline
{
}

internal sealed class ValidationPipeline : IValidationPipeline
{
    private readonly FrozenDictionary<Type, ImmutableArray<Validator>> _lookup;
    private readonly IServiceProvider _services;

    public ValidationPipeline(ValidationPipelineBuilder builder, IServiceProvider provider)
    {
        _services = provider;
        _lookup = BuildPipeline(builder, provider).ToFrozenDictionary();
    }

    private async Task<OneOf>

    private static ImmutableArray<Validator> BuildFromRegistrator(KeyValuePair<Type, PipelineRegistrator> kvp, IServiceProvider services)
    {
        ImmutableArray<Type> stepTypes = kvp.Value.GetSteps();
        Validator[] buffer = ArrayPool<Validator>.Shared.Rent(stepTypes.Length);

        for (int i = 0; i < stepTypes.Length; i++)
        {
            Validator step = (Validator)services.GetRequiredService(stepTypes[i]);
            buffer[i] = step;
        }

        ImmutableArray<Validator> array = ImmutableArray.Create(buffer.AsSpan(0, stepTypes.Length));
        ArrayPool<Validator>.Shared.Return(buffer);

        return array;
    }
    private static IEnumerable<KeyValuePair<Type, ImmutableArray<Validator>>> BuildPipeline(ValidationPipelineBuilder builder, IServiceProvider services)
    {
        foreach (var kvp in builder)
        {
            ImmutableArray<Validator> steps = BuildFromRegistrator(kvp, services);
            yield return new(kvp.Key, steps);
        }
    }
}
