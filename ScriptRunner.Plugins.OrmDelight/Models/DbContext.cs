using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;
using ScriptRunner.Plugins.OrmDelight.Attributes;

namespace ScriptRunner.Plugins.OrmDelight.Models;

/// <summary>
///     Provides a context for managing database operations, including schema creation, validation, and query execution.
/// </summary>
public class DbContext
{
    private readonly IDbConnection _dbConnection;

    /// <summary>
    /// Cache for column definitions, mapped by type.
    /// </summary>
    private static readonly Dictionary<Type, List<string>> ColumnCache = new();

    /// <summary>
    /// Cache for foreign key definitions, mapped by type.
    /// </summary>
    private static readonly Dictionary<Type, List<string>> ForeignKeyCache = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="DbContext" /> class.
    /// </summary>
    /// <param name="dbConnection">The database connection to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dbConnection" /> is null.</exception>
    public DbContext(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
    }

    /// <summary>
    ///     Registers a model type and ensures the corresponding table exists in the database.
    /// </summary>
    /// <typeparam name="T">The type of the model to register.</typeparam>
    /// <param name="tableName">The name of the table to create or ensure existence.</param>
    public void RegisterModel<T>(string tableName)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        var type = typeof(T);
        var columns = GetColumns(type);
        var foreignKeys = GetForeignKeys(type);

        // Check for CompositeKeyAttribute
        var compositeKeyAttr = type.GetCustomAttribute<CompositeKeyAttribute>();
        string? compositeKey = null;
        if (compositeKeyAttr != null) compositeKey = $"PRIMARY KEY ({string.Join(", ", compositeKeyAttr.Columns)})";

        var allConstraints = columns.Concat(foreignKeys);
        if (compositeKey != null) allConstraints = allConstraints.Append(compositeKey);

        var createTableQuery = $"""
                                CREATE TABLE IF NOT EXISTS {tableName} (
                                    {string.Join(", ", allConstraints)}
                                );
                                """;

        _dbConnection.Execute(createTableQuery);
    }

    /// <summary>
    ///     Executes an SQL query and returns the results as a collection of strongly typed objects.
    /// </summary>
    /// <typeparam name="T">The type of the result set objects.</typeparam>
    /// <param name="sql">The SQL query to execute.</param>
    /// <param name="parameters">The parameters for the query, if any.</param>
    /// <returns>A collection of objects of type <typeparamref name="T" />.</returns>
    public IEnumerable<T> Query<T>(string sql, object? parameters = null)
    {
        try
        {
            EnsureConnection();
            return _dbConnection.Query<T>(sql, parameters);
        }
        finally
        {
            CloseConnection();
        }
    }

    /// <summary>
    ///     Executes an SQL query and returns the results as a collection of dynamic objects.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <param name="parameters">The parameters for the query, if any.</param>
    /// <returns>A collection of dynamic objects representing the result set.</returns>
    public IEnumerable<dynamic> Query(string sql, object? parameters = null)
    {
        EnsureConnection();
        return _dbConnection.Query(sql, parameters); // Dapper its built-in support for dynamic queries
    }

    /// <summary>
    ///     Executes an SQL query and returns a single result or the default value if no results exist.
    /// </summary>
    /// <typeparam name="T">The type of the result object.</typeparam>
    /// <param name="sql">The SQL query to execute.</param>
    /// <param name="parameters">The parameters for the query, if any.</param>
    /// <returns>An object of type <typeparamref name="T" /> or default if no result is found.</returns>
    public T? QuerySingleOrDefault<T>(string sql, object? parameters = null)
    {
        EnsureConnection();
        return _dbConnection.QuerySingleOrDefault<T>(sql, parameters);
    }

    /// <summary>
    /// Executes an SQL command and returns the number of affected rows.
    /// </summary>
    /// <param name="sql">The SQL command to execute.</param>
    /// <param name="parameters">The parameters for the command, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <returns>The number of rows affected by the command.</returns>
    public int Execute(string sql, object? parameters = null, IDbTransaction? transaction = null)
    {
        EnsureConnection();
        return _dbConnection.Execute(sql, parameters, transaction);
    }

    /// <summary>
    ///     Executes a block of operations within a database transaction.
    /// </summary>
    /// <param name="transactionBlock">
    ///     A delegate that performs operations using the provided <see cref="IDbTransaction" />.
    /// </param>
    /// <exception cref="Exception">Re-throws any exceptions that occur within the transaction.</exception>
    public void ExecuteInTransaction(Action<IDbTransaction> transactionBlock)
    {
        EnsureConnection();
        using var transaction = _dbConnection.BeginTransaction();
        try
        {
            transactionBlock(transaction);
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
        finally
        {
            CloseConnection();
        }
    }
    
    /// <summary>
    /// Executes an SQL command and returns a scalar result.
    /// </summary>
    /// <typeparam name="T">The type of the scalar result.</typeparam>
    /// <param name="sql">The SQL command to execute.</param>
    /// <param name="parameters">The parameters for the command, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <returns>A scalar value of type <typeparamref name="T" />.</returns>
    public T? ExecuteScalar<T>(string sql, object? parameters = null, IDbTransaction? transaction = null)
    {
        EnsureConnection();
        return _dbConnection.ExecuteScalar<T>(sql, parameters, transaction);
    }

    /// <summary>
    ///     Validates an entity's properties based on attributes.
    /// </summary>
    /// <typeparam name="T">The type of the entity to validate.</typeparam>
    /// <param name="entity">The entity to validate.</param>
    /// <exception cref="InvalidOperationException">Thrown if validation fails.</exception>
    public void Validate<T>(T entity)
    {
        EnsureConnection();

        var type = typeof(T);
        var compositeKeyAttr = type.GetCustomAttribute<CompositeKeyAttribute>();

        if (compositeKeyAttr != null)
        {
            var tableName = type.Name;
            var conditions = string.Join(" AND ", compositeKeyAttr.Columns.Select(col => $"{col} = @{col}"));
            var query = $"SELECT COUNT(*) FROM {tableName} WHERE {conditions}";

            var parameters =
                compositeKeyAttr.Columns.ToDictionary(col => col, col => type.GetProperty(col)?.GetValue(entity));
            var count = _dbConnection.ExecuteScalar<int>(query, parameters);

            if (count > 0) throw new InvalidOperationException($"Composite key violation on table {tableName}.");
        }

        // Continue with existing validation logic for individual attributes
        foreach (var property in type.GetProperties())
        {
            var value = property.GetValue(entity);

            // Check RequiredAttribute
            var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
            if (requiredAttribute != null && (value == null || string.IsNullOrWhiteSpace(value.ToString())))
                throw new InvalidOperationException(requiredAttribute.ErrorMessage);

            // Check UniqueAttribute
            var uniqueAttribute = property.GetCustomAttribute<UniqueAttribute>();
            if (uniqueAttribute == null) continue;

            var columnName = property.Name;
            var query = $"SELECT COUNT(*) FROM {type.Name} WHERE {columnName} = @Value";
            var count = _dbConnection.ExecuteScalar<int>(query, new { Value = value });

            if (count > 0) throw new InvalidOperationException(uniqueAttribute.ErrorMessage);
        }
    }

    /// <summary>
    ///     Ensures that the database connection is open.
    /// </summary>
    private void EnsureConnection()
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();
    }

    /// <summary>
    ///     Ensures that the database connection is closed.
    /// </summary>
    private void CloseConnection()
    {
        if (_dbConnection.State == ConnectionState.Open)
            _dbConnection.Close();
    }

    /// <summary>
    ///     Generates the SQL column definitions based on a type's properties and attributes, with caching.
    /// </summary>
    /// <param name="type">The type to analyze.</param>
    /// <returns>A collection of SQL column definitions.</returns>
    private static IEnumerable<string> GetColumns(Type type)
    {
        if (ColumnCache.TryGetValue(type, out var cachedColumns)) return cachedColumns;

        cachedColumns = type.GetProperties()
            .Select(property =>
            {
                var columnDefinition = $"{property.Name} {MapTypeToSql(property.PropertyType)}";

                var primaryKeyAttr = property.GetCustomAttribute<PrimaryKeyAttribute>();
                if (primaryKeyAttr != null)
                {
                    columnDefinition += " PRIMARY KEY";
                    if (primaryKeyAttr.AutoIncrement)
                        columnDefinition += " AUTOINCREMENT";
                }

                if (property.GetCustomAttribute<UniqueAttribute>() != null)
                    columnDefinition += " UNIQUE";

                if (property.GetCustomAttribute<RequiredAttribute>() != null)
                    columnDefinition += " NOT NULL";

                return columnDefinition;
            })
            .ToList();

        ColumnCache[type] = cachedColumns;

        return cachedColumns;
    }

    /// <summary>
    ///     Generates SQL foreign key constraints based on a type's properties and attributes, with caching.
    /// </summary>
    /// <param name="type">The type to analyze.</param>
    /// <returns>A collection of SQL foreign key definitions.</returns>
    private static IEnumerable<string> GetForeignKeys(Type type)
    {
        if (ForeignKeyCache.TryGetValue(type, out var cachedForeignKeys)) return cachedForeignKeys;

        cachedForeignKeys = type.GetProperties()
            .Where(p => p.GetCustomAttribute<ForeignKeyAttribute>() != null)
            .Select(property =>
            {
                var foreignKeyAttr = property.GetCustomAttribute<ForeignKeyAttribute>();
                return
                    $"FOREIGN KEY ({property.Name}) REFERENCES {foreignKeyAttr?.ReferencedTable}({foreignKeyAttr?.ReferencedColumn})";
            })
            .ToList();

        ForeignKeyCache[type] = cachedForeignKeys;

        return cachedForeignKeys;
    }

    /// <summary>
    ///     Maps a .NET type to its corresponding SQL type.
    /// </summary>
    /// <param name="type">The .NET type to map.</param>
    /// <returns>The SQL type as a string.</returns>
    /// <exception cref="NotSupportedException">Thrown if the .NET type is not supported.</exception>
    private static string MapTypeToSql(Type type)
    {
        return type switch
        {
            { } t when t == typeof(int) => "INTEGER",
            { } t when t == typeof(string) => "TEXT",
            { } t when t == typeof(DateTime) => "DATETIME",
            { } t when t == typeof(bool) => "BOOLEAN",
            { } t when t == typeof(double) => "REAL",
            { } t when t == typeof(decimal) => "NUMERIC",
            _ => throw new NotSupportedException($"Type '{type}' is not supported.")
        };
    }
}