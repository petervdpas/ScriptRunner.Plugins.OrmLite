using System;

namespace ScriptRunner.Plugins.OrmLite.Interfaces;

/// <summary>
/// Defines the contract for SQL dialects used in the OrmLite framework.
/// </summary>
public interface ISqlDialect
{
    /// <summary>
    /// Maps a .NET type to its equivalent SQL type for the specific SQL dialect.
    /// </summary>
    /// <param name="type">The .NET type to map.</param>
    /// <returns>A string representing the SQL type in the specific dialect.</returns>
    /// <exception cref="NotSupportedException">Thrown if the provided .NET type is not supported by the dialect.</exception>
    string MapType(Type type);

    /// <summary>
    /// Gets the syntax for defining a primary key in the specific SQL dialect.
    /// </summary>
    string PrimaryKeySyntax { get; }

    /// <summary>
    /// Gets the syntax for defining an auto-incrementing column in the specific SQL dialect.
    /// </summary>
    string AutoIncrementSyntax { get; }

    /// <summary>
    /// Gets the syntax for defining a unique constraint in the specific SQL dialect.
    /// </summary>
    string UniqueSyntax { get; }

    /// <summary>
    /// Gets the syntax for defining a non-null constraint in the specific SQL dialect.
    /// </summary>
    string NotNullSyntax { get; }
}
