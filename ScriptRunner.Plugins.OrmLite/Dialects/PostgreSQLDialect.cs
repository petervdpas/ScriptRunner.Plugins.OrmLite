using System;
using ScriptRunner.Plugins.OrmLite.Interfaces;

namespace ScriptRunner.Plugins.OrmLite.Dialects;

/// <summary>
///     Provides SQL syntax definitions and type mappings for PostgreSQL.
/// </summary>
public class PostgreSQLDialect : ISqlDialect
{
    /// <summary>
    ///     Maps a .NET type to its equivalent PostgreSQL type.
    /// </summary>
    /// <param name="type">The .NET type to map.</param>
    /// <returns>A string representing the PostgreSQL type.</returns>
    /// <exception cref="NotSupportedException">Thrown if the provided .NET type is not supported.</exception>
    public string MapType(Type type)
    {
        return type switch
        {
            not null when type == typeof(int) => "INTEGER",
            not null when type == typeof(string) => "TEXT",
            not null when type == typeof(DateTime) => "TIMESTAMP",
            not null when type == typeof(bool) => "BOOLEAN",
            not null when type == typeof(double) => "DOUBLE PRECISION",
            not null when type == typeof(decimal) => "NUMERIC(18,2)",
            _ => throw new NotSupportedException($"Type '{type}' is not supported.")
        };
    }

    /// <summary>
    ///     Gets the syntax for defining a primary key in PostgreSQL.
    /// </summary>
    public string PrimaryKeySyntax => "PRIMARY KEY";

    /// <summary>
    ///     Gets the syntax for defining an auto-incrementing column in PostgreSQL.
    /// </summary>
    public string AutoIncrementSyntax => "SERIAL";

    /// <summary>
    ///     Gets the syntax for defining a unique constraint in PostgreSQL.
    /// </summary>
    public string UniqueSyntax => "UNIQUE";

    /// <summary>
    ///     Gets the syntax for defining a non-null constraint in PostgreSQL.
    /// </summary>
    public string NotNullSyntax => "NOT NULL";

    /// <summary>
    ///     Gets the query to retrieve the ID of the last inserted row in PostgreSQL.
    /// </summary>
    /// <param name="tableName">The name of the table to retrieve the sequence from.</param>
    /// <returns>A SQL query string to retrieve the last inserted row ID using PostgreSQL's sequence mechanism.</returns>
    public string GetLastInsertIdQuery(string tableName)
    {
        return $"SELECT CURRVAL(pg_get_serial_sequence('{tableName}', 'id'));";
    }
}