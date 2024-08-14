using Microsoft.Graph;

namespace Yev.Bonobo.Database;

public readonly struct DbExpressionState<T> : IAsyncDisposable, IDisposable, IServiceProvider
{
    public readonly CancellationToken CancellationToken;
    public readonly T State;

    public DbExpressionState(in T state)
    {
        State = state;
        CancellationToken = CancellationToken.None;
    }
    public DbExpressionState(T state, CancellationToken cancellationToken)
    {
        State = state;
        CancellationToken = cancellationToken;
    }

    public static implicit operator DbExpressionState<T>(T state)
    {
        return new(state);
    }
    public static implicit operator T(DbExpressionState<T> state)
    {
        return state.State;
    }
    public static implicit operator DbExpressionState<T>((CancellationToken token, T state) tuple)
    {
        return new(tuple.state, tuple.token);
    }
    public static implicit operator DbExpressionState<T>((T state, CancellationToken token) tuple)
    {
        return new(tuple.state, tuple.token);
    }
}