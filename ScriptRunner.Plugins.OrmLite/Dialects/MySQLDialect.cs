using System;
using ScriptRunner.Plugins.OrmLite.Interfaces;

namespace ScriptRunner.Plugins.OrmLite.Dialects;

/// <summary>
///     Provides MySQL-specific SQL syntax and type mapping for the OrmLite framework.
/// </summary>
public class MySQLDialect : ISqlDialect
{
    /// <summary>
    ///     Maps a .NET type to its equivalent MySQL SQL type.
    /// </summary>
    /// <param name="type">The .NET type to map.</param>
    /// <returns>A string representing the MySQL type.</returns>
    /// <exception cref="NotSupportedException">Thrown if the provided .NET type is not supported.</exception>
    public string MapType(Type type)
    {
        return type switch
        {
            not null when type == typeof(int) => "INT",
            not null when type == typeof(string) => "VARCHAR(255)",
            not null when type == typeof(DateTime) => "DATETIME",
            not null when type == typeof(bool) => "TINYINT(1)",
            not null when type == typeof(double) => "DOUBLE",
            not null when type == typeof(decimal) => "DECIMAL(18,2)",
            _ => throw new NotSupportedException($"Type '{type}' is not supported.")
        };
    }

    /// <summary>
    ///     Gets the syntax for defining a primary key in MySQL.
    /// </summary>
    public string PrimaryKeySyntax => "PRIMARY KEY";

    /// <summary>
    ///     Gets the syntax for defining an auto-incrementing column in MySQL.
    /// </summary>
    public string AutoIncrementSyntax => "AUTO_INCREMENT";

    /// <summary>
    ///     Gets the syntax for defining a unique constraint in MySQL.
    /// </summary>
    public string UniqueSyntax => "UNIQUE";

    /// <summary>
    ///     Gets the syntax for defining a non-null constraint in MySQL.
    /// </summary>
    public string NotNullSyntax => "NOT NULL";

    /// <summary>
    ///     Gets the query to retrieve the ID of the last inserted row in MySQL.
    /// </summary>
    /// <param name="tableName">The name of the table (unused in MySQL's LAST_INSERT_ID mechanism).</param>
    /// <returns>A SQL query string to retrieve the last inserted row ID using MySQL's LAST_INSERT_ID function.</returns>
    public string GetLastInsertIdQuery(string tableName)
    {
        return "SELECT LAST_INSERT_ID();";
    }
}