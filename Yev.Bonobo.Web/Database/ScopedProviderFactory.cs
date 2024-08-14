using AttributeDI.Attributes;

namespace Yev.Bonobo.Database;

public interface IScopedProviderFactory : IServiceScopeFactory
{
    AsyncScopedProvider CreateOptionalAsyncScope(IServiceProvider? scopedProvider);
}

[ServiceRegistration(typeof(IScopedProviderFactory))]
internal sealed class OptionalScopeProviderFactory : IScopedProviderFactory
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OptionalScopeProviderFactory(IServiceScopeFactory factory)
    {
        _scopeFactory = factory;
    }

    public IServiceScope CreateScope()
    {
        return _scopeFactory.CreateScope();
    }
    public AsyncScopedProvider CreateOptionalAsyncScope(IServiceProvider? scopedProvider)
    {
        if (scopedProvider is not null)
        {
            return new AsyncScopedProvider(scopedProvider);
        }

        AsyncServiceScope asyncScope = _scopeFactory.CreateAsyncScope();
        return new AsyncScopedProvider(in asyncScope);
    }
}

public readonly struct AsyncScopedProvider : IAsyncDisposable, IServiceScope
{
    private readonly bool _isNotDefault;
    private readonly bool _existingProvider;
    private readonly IServiceProvider _provider;
    private readonly AsyncServiceScope _scope;

    public readonly bool NeedsDisposal => !_existingProvider;
    public readonly IServiceProvider ServiceProvider => _provider;

    internal AsyncScopedProvider(IServiceProvider existingProvider)
    {
        _isNotDefault = true;
        _existingProvider = true;
        _provider = existingProvider;
        _scope = new AsyncServiceScope();
    }
    internal AsyncScopedProvider(in AsyncServiceScope asyncScope)
    {
        _isNotDefault = true;
        _existingProvider = false;
        _scope = asyncScope;
        _provider = asyncScope.ServiceProvider;
    }

    public readonly void Dispose()
    {
        if (_isNotDefault)
        {
            if (!_existingProvider)
            {
                _scope.Dispose();
            }
        }
    }
    public readonly ValueTask DisposeAsync()
    {
        if (_isNotDefault)
        {
            if (!_existingProvider)
            {
                return _scope.DisposeAsync();
            }
        }

        return ValueTask.CompletedTask;
    }
}