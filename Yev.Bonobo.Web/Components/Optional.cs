namespace Yev.Bonobo;

public static class Optional
{
    /// <summary>
    /// Creates an <see cref="Optional{T}"/> from a value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Optional<T> FromValue<T>([DisallowNull] T value)
    {
        return new(value);
    }
}

/// <summary>
/// Represents a value that may or may not be present.
/// </summary>
/// <typeparam name="T">The type of value that may be present.</typeparam>
public readonly struct Optional<T>
{
    private readonly bool _isNotNull;
    private readonly T? _value;

    /// <summary>
    /// Indicates that the <see cref="Optional{T}"/> contains a value.
    /// </summary>
    [MemberNotNullWhen(true, nameof(_value), nameof(Value))]
    public bool HasValue => _isNotNull;
    /// <summary>
    /// The value the <see cref="Optional{T}"/> contains if one was provided - or default or
    /// <see langword="null"/> if not.
    /// </summary>
    public T? Value => _value;

    public Optional([DisallowNull] T value)
    {
        _value = value;
        _isNotNull = true;
    }
}