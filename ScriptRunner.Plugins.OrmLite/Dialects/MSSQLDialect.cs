using System;
using ScriptRunner.Plugins.OrmLite.Interfaces;

namespace ScriptRunner.Plugins.OrmLite.Dialects;

/// <summary>
/// Provides SQL syntax definitions and type mappings for Microsoft SQL Server.
/// </summary>
public class MSSQLDialect : ISqlDialect
{
    /// <summary>
    /// Maps a .NET type to its equivalent SQL Server type.
    /// </summary>
    /// <param name="type">The .NET type to map.</param>
    /// <returns>A string representing the SQL Server type.</returns>
    /// <exception cref="NotSupportedException">Thrown if the provided .NET type is not supported.</exception>
    public string MapType(Type type) => type switch
    {
        not null when type == typeof(int) => "INT",
        not null when type == typeof(string) => "NVARCHAR(MAX)",
        not null when type == typeof(DateTime) => "DATETIME",
        not null when type == typeof(bool) => "BIT",
        not null when type == typeof(double) => "FLOAT",
        not null when type == typeof(decimal) => "DECIMAL(18,2)",
        _ => throw new NotSupportedException($"Type '{type}' is not supported.")
    };

    /// <summary>
    /// Gets the syntax for defining a primary key in Microsoft SQL Server.
    /// </summary>
    public string PrimaryKeySyntax => "PRIMARY KEY";

    /// <summary>
    /// Gets the syntax for defining an auto-incrementing column in Microsoft SQL Server.
    /// </summary>
    public string AutoIncrementSyntax => "IDENTITY(1,1)";

    /// <summary>
    /// Gets the syntax for defining a unique constraint in Microsoft SQL Server.
    /// </summary>
    public string UniqueSyntax => "UNIQUE";

    /// <summary>
    /// Gets the syntax for defining a non-null constraint in Microsoft SQL Server.
    /// </summary>
    public string NotNullSyntax => "NOT NULL";
}