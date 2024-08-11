namespace Yev.Bonobo.Git;

public abstract class ViolatileGitObject : IDisposable
{
    private bool _disposed;
    protected abstract IDisposable Violatile { get; }

    #region DISPOSAL
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                this.Violatile?.Dispose();
            }

            _disposed = true;
        }
    }

    /// <inheritdoc cref="ObjectDisposedException.ThrowIf(bool, object)"/>
    protected void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    #endregion
}