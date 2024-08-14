using Yev.Bonobo.Validation.Pipelines;

namespace Yev.Bonobo.Validation;

/// <summary>
/// Enumeration values describing the type of operation being requested to perform. Used by 
/// <see cref="IValidationPipeline"/> implementations to provide filtering logic.
/// </summary>
[Flags]
public enum OperationType
{
    /// <summary>
    /// An invalid type of operation.
    /// </summary>
    None = 0,
    /// <summary>
    /// The type of operation has not been specified. Rarely used as all validations will be performed.
    /// </summary>
    Unspecified = 1,
    /// <summary>
    /// Describes a read-only type of operation, e.g. - a non-tracked EFCore SQL query.
    /// </summary>
    ReadOnly = 2,
    /// <summary>
    /// Describes that the operation is updating an existing entity in the database.
    /// </summary>
    Update = 4,
    /// <summary>
    /// Describes that the operation is inserting/creating a new entity that does not exist in 
    /// the database.
    /// </summary>
    Create = 8,
    /// <summary>
    /// Describes that the the operation is removing/deleting/dropping an existing entity in 
    /// the database.
    /// </summary>
    Removal = 16,
    /// <summary>
    /// Used to indicate the operation is related to merging two or more entities together.
    /// </summary>
    Merge = 64,

    /// <summary>
    /// Used to indicate that the operation does not insert or delete any existing other records and 
    /// thus only changes metadata scoped to the current entity.
    /// </summary>
    SafeUpdate = 2048,
}