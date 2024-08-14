using AttributeDI.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Yev.Bonobo.Validation;

/// <summary>
///     Describes the context in which a database entity validation is being performed.
/// </summary>
/// <remarks>
///     This class contains information describing the instance on which
///     validation is being performed.
///     <para>
///         It supports <see cref="IServiceProvider" /> so that custom validation
///         code can acquire additional services to help it perform its validation.
///     </para>
///     <para>
///         An <see cref="Items" /> property bag is available for additional contextual
///         information about the validation.  Values stored in <see cref="Items" />
///         will be available to validation methods that use this <see cref="DbValidationContext" />.
///     </para>
/// </remarks>
public sealed class DbValidationContext : IServiceProvider
{
    private static readonly ObjectEqualityComparer _comparer = new();
    private readonly List<ValidationResult> _errors;
    private Dictionary<object, object>? _items;
    private readonly IServiceProvider _provider;

    public bool HasErrors => _errors.Count > 0;

    /// <summary>
    ///     Gets the dictionary of key/value pairs associated with this context.
    /// </summary>
    /// <value>
    ///     This property will never be <see langword="null"/>, but the dictionary may be empty.
    /// </value>
    public IDictionary<object, object> Items => _items ??= new(_comparer);

    /// <summary>
    ///     The object instance being validated.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     During validation, especially property-level validation, the object instance might be in an 
    ///     indeterminate state.<br/>
    ///     For example, the property being validated, as well as other properties on the instance might not have been
    ///     updated to their new values.
    ///     </para>
    /// </remarks>
    /// <value>
    ///     While it will not be <see langword="null"/>, the state of the instance is indeterminate
    ///     as it might only be partially initialized during validation.
    /// </value>
    public object ObjectInstance { get; }
    /// <summary>
    ///     The type of the object being validated.
    /// </summary>
    /// <remarks>
    ///     This will never be <see langword="null"/>, but the type may be a base <see cref="Type"/> of
    ///     <see cref="ObjectInstance"/> instead of the direct type.
    /// </remarks>
    public Type ObjectType { get; }

    /// <summary>
    /// The pre-defined type of operation the context is executing for.
    /// </summary>
    /// <remarks>
    ///     This is a bit flag value meaning that this context could be in use for multiple types.
    /// </remarks>
    public OperationType OperationType { get; }

    /// <summary>
    /// Gets or sets whether this context should signal to the pipeline validation should stop and return as-is.
    /// </summary>
    /// <remarks>
    ///     Used as a way to short-circuit the validation process. Careful using it.
    /// </remarks>
    public bool ShouldNotContinue { get; set; }

    /// <summary>
    ///     Construct a <see cref="DbValidationContext"/> for a given object instance, its type, 
    ///     and an <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="instance">The object instance being validated.  It cannot be <see langword="null"/>.</param>
    /// <param name="instanceType"><inheritdoc cref="ObjectType" path="/summary"/></param>
    /// <param name="provider">
    ///     Optional <see cref="IServiceProvider" /> to use when <see cref="GetService" /> is called.
    ///     If it is <see langword="null"/>, <see cref="GetService" /> will always return <see langword="null"/>.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="operationType"/> is less than or equal to 0 or an invalid combination.
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="instance" /> is null.</exception>
    public DbValidationContext(object instance, Type instanceType, OperationType operationType, IServiceProvider provider)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero((int)operationType, nameof(operationType));
        ArgumentNullException.ThrowIfNull(instance);

        _errors = [];
        _provider = provider;
        this.OperationType = operationType;
        this.ObjectInstance = instance;
        this.ObjectType = instanceType;
    }
    /// <summary>
    ///     Construct a <see cref="DbValidationContext" /> for a given object instance, its type, 
    ///     an <see cref="IServiceProvider" />, and an optional property bag of <paramref name="items" />.
    /// </summary>
    /// <param name="instance">The object instance being validated.  It cannot be <see langword="null"/>.</param>
    /// <param name="instanceType"><inheritdoc cref="ObjectType" path="/summary"/></param>
    /// <param name="provider">
    ///     Optional <see cref="IServiceProvider" /> to use when <see cref="GetService" /> is called.
    ///     If it is <see langword="null"/>, <see cref="GetService" /> will always return <see langword="null"/>.
    /// </param>
    /// <param name="items">
    ///     Optional set of key/value pairs to make available to consumers via <see cref="Items" />.
    ///     If <see langword="null"/>, an empty dictionary will be created.  
    ///     If not <see langword="null"/>, the set of key/value pairs will be copied into a
    ///     new dictionary, preventing consumers from modifying the original dictionary.
    /// </param>
    /// <inheritdoc cref="DbValidationContext(object, Type, OperationType, IServiceProvider)" path="/exception"/>
    public DbValidationContext(object instance, Type instanceType, OperationType operationType, IServiceProvider provider, IDictionary<object, object>? items)
        : this(instance, instanceType, operationType, provider)
    {
        _items = items is not null
            ? new(items, _comparer)
            : null;
    }

    public void AddError(ValidationResult result)
    {
        _errors.Add(result);
    }
    public void AddError(string errorMessage)
    {
        this.AddError(errorMessage, memberNames: Array.Empty<string>());
    }
    public void AddError(string errorMessage, IEnumerable<string>? memberNames)
    {
        this.AddError(new ValidationResult(errorMessage, memberNames));
    }

    public object? GetService(Type serviceType)
    {
        return _provider.GetService(serviceType);
    }

    /// <summary>
    ///     Construct a <see cref="DbValidationContext" /> for a given object instance and an
    ///     <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="instance">The object instance being validated.  It cannot be <see langword="null"/>.</param>
    /// <param name="type"><inheritdoc cref="OperationType" path="/summary"/></param>
    /// <param name="provider">
    ///     Optional <see cref="IServiceProvider" /> to use when <see cref="GetService" /> is called.
    ///     If it is <see langword="null"/>, <see cref="GetService" /> will always return <see langword="null"/>.
    /// </param>
    /// <inheritdoc cref="DbValidationContext(object, Type, OperationType, IServiceProvider)" path="/exception"/>
    public static DbValidationContext Create<T>([DisallowNull] T instance, OperationType type, IServiceProvider provider)
    {
        return new(instance, typeof(T), type, provider);
    }
    /// <summary>
    ///     Construct a <see cref="DbValidationContext"/> for a given object instance, an
    ///     <see cref="IServiceProvider"/>, and an optional property bag of <paramref name="items"/>.
    /// </summary>
    /// <param name="instance"><inheritdoc cref="ObjectInstance" path="/summary"/>.</param>
    /// <param name="type"><inheritdoc cref="OperationType" path="/summary"/></param>
    /// <param name="provider">
    ///     Optional <see cref="IServiceProvider" /> to use when <see cref="GetService"/> is called.
    ///     If it is <see langword="null"/>, <see cref="GetService" /> will always return <see langword="null"/>.
    /// </param>
    /// <param name="items">
    ///     Optional set of key/value pairs to make available to consumers via <see cref="Items"/>.
    ///     If <see langword="null"/>, an empty dictionary will be created.  
    ///     If not <see langword="null"/>, the set of key/value pairs will be copied into a
    ///     new dictionary, preventing consumers from modifying the original dictionary.
    /// </param>
    /// <inheritdoc cref="DbValidationContext(object, Type, OperationType, IServiceProvider)" path="/exception"/>
    public static DbValidationContext Create<T>([DisallowNull] T instance, OperationType type, IServiceProvider provider, IDictionary<object, object>? items)
    {
        return items is null
            ? new(instance, typeof(T), type, provider)
            : new(instance, typeof(T), type, provider, items);
    }

    private sealed class ObjectEqualityComparer : IEqualityComparer<object>
    {
        private static readonly StringComparer _comparer = StringComparer.OrdinalIgnoreCase;
        public new bool Equals(object? x, object? y)
        {
            return _comparer.Equals(x, y);
        }
        public int GetHashCode([DisallowNull] object obj)
        {
            return _comparer.GetHashCode(obj);
        }
    }
}