using System;
using ScriptRunner.Plugins.OrmLite.Interfaces;

namespace ScriptRunner.Plugins.OrmLite.Dialects;

/// <summary>
/// Provides SQLite-specific SQL syntax and type mapping for the OrmLite framework.
/// </summary>
public class SQLiteDialect : ISqlDialect
{
    /// <summary>
    /// Maps a .NET type to its equivalent SQLite SQL type.
    /// </summary>
    /// <param name="type">The .NET type to map.</param>
    /// <returns>A string representing the SQLite type.</returns>
    /// <exception cref="NotSupportedException">Thrown if the provided .NET type is not supported.</exception>
    public string MapType(Type type) => type switch
    {
        not null when type == typeof(int) => "INTEGER",
        not null when type == typeof(string) => "TEXT",
        not null when type == typeof(DateTime) => "DATETIME",
        not null when type == typeof(bool) => "BOOLEAN",
        not null when type == typeof(double) => "REAL",
        not null when type == typeof(decimal) => "NUMERIC",
        _ => throw new NotSupportedException($"Type '{type}' is not supported.")
    };

    /// <summary>
    /// Gets the syntax for defining a primary key in SQLite.
    /// </summary>
    public string PrimaryKeySyntax => "PRIMARY KEY";

    /// <summary>
    /// Gets the syntax for defining an auto-incrementing column in SQLite.
    /// </summary>
    public string AutoIncrementSyntax => "AUTOINCREMENT";

    /// <summary>
    /// Gets the syntax for defining a unique constraint in SQLite.
    /// </summary>
    public string UniqueSyntax => "UNIQUE";

    /// <summary>
    /// Gets the syntax for defining a non-null constraint in SQLite.
    /// </summary>
    public string NotNullSyntax => "NOT NULL";
    
    /// <summary>
    /// Gets the query to retrieve the ID of the last inserted row in SQLite.
    /// </summary>
    /// <param name="tableName">The name of the table (not used in SQLite's syntax).</param>
    /// <returns>A SQL query string to retrieve the last inserted row ID.</returns>
    public string GetLastInsertIdQuery(string tableName) => "SELECT last_insert_rowid();";
}