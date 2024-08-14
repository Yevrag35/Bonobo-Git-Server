using AttributeDI.Attributes;

namespace Yev.Bonobo.Database;

public interface IExpressionStateFactory
{
    DbExpressionState<T> Create<T>(T state, CancellationToken token);
    DbExpressionState<T> Create<T>(T state, HttpContext request);
}

[ServiceRegistration(typeof(IExpressionStateFactory))]
internal sealed class ExpressionStateFactory : IExpressionStateFactory
{
    private readonly IServiceProvider _provider;

    public ExpressionStateFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public DbExpressionState<T> Create<T>(T state, CancellationToken token)
    {
        AsyncServiceScope scope = _provider.CreateAsyncScope();
        return new(state, scope, token);
    }
    public DbExpressionState<T> Create<T>(T state, HttpContext request)
    {
        return new(state, request.RequestServices, request.RequestAborted);
    }
}