using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using ScriptRunner.Plugins.OrmLite.Attributes;
using ScriptRunner.Plugins.OrmLite.Interfaces;
using ScriptRunner.Plugins.OrmLite.Models;

namespace ScriptRunner.Plugins.OrmLite;

/// <summary>
///     Generic data service that provides CRUD operations and dynamic query execution.
/// </summary>
public class OrmLite : IOrmLite
{
    private DbContext? _dbContext;
    private ISqlDialect? _sqlDialect;

    /// <summary>
    ///     Sets the database connection and SQL dialect to be used by the service.
    /// </summary>
    /// <param name="dbConnection">The database connection to use.</param>
    /// <param name="sqlDialect">The SQL dialect to use for schema and query generation.</param>
    public void Initialize(IDbConnection dbConnection, ISqlDialect sqlDialect)
    {
        _dbContext = new DbContext(dbConnection ?? throw new ArgumentNullException(nameof(dbConnection)));
        _sqlDialect = sqlDialect ?? throw new ArgumentNullException(nameof(sqlDialect));
    }

    /// <summary>
    ///     Registers a model type with the service and ensures the database schema matches.
    /// </summary>
    /// <typeparam name="T">The type of the model to register.</typeparam>
    /// <param name="tableName">
    ///     The name of the table for the model. If null, the table name is inferred from the <see cref="TableAttribute" />.
    /// </param>
    public void RegisterModel<T>(string? tableName = null)
    {
        EnsureInitialized();
        _dbContext!.RegisterModel<T>(_sqlDialect!, tableName);
    }

    /// <summary>
    ///     Validates an entity's properties based on attributes.
    /// </summary>
    /// <typeparam name="T">The type of the entity to validate.</typeparam>
    /// <param name="entity">The entity to validate.</param>
    /// <exception cref="InvalidOperationException">Thrown if validation fails.</exception>
    public void Validate<T>(T entity)
    {
        EnsureInitialized();
        _dbContext!.Validate(entity);
    }

    /// <summary>
    ///     Retrieves all records of a given type from a specified table.
    /// </summary>
    /// <typeparam name="T">The type of the records to retrieve.</typeparam>
    /// <param name="tableName">The name of the table to query.</param>
    /// <returns>A list of records of type <typeparamref name="T" />.</returns>
    public IEnumerable<T> GetAll<T>(string tableName)
    {
        EnsureInitialized();
        var query = $"SELECT * FROM {tableName}";
        return _dbContext!.Query<T>(query);
    }

    /// <summary>
    ///     Retrieves a record by its primary key from a specified table.
    /// </summary>
    /// <typeparam name="T">The type of the record to retrieve.</typeparam>
    /// <param name="tableName">The name of the table to query.</param>
    /// <param name="idColumn">The name of the primary key column.</param>
    /// <param name="idValue">The value of the primary key to search for.</param>
    /// <returns>The matching record of type <typeparamref name="T" />, or null if not found.</returns>
    public T? GetById<T>(string tableName, string idColumn, object idValue)
    {
        EnsureInitialized();
        var query = $"SELECT * FROM {tableName} WHERE {idColumn} = @Id";
        return _dbContext!.QuerySingleOrDefault<T>(query, new { Id = idValue });
    }

    /// <summary>
    ///     Inserts a new record into a specified table and returns the generated ID within a transaction.
    /// </summary>
    /// <typeparam name="T">The type of the record to insert.</typeparam>
    /// <param name="tableName">The name of the table to insert into.</param>
    /// <param name="entity">The record to insert.</param>
    /// <param name="transaction">
    ///     The database transaction to use when executing the command.
    ///     If null, the operation is executed without a transaction.
    /// </param>
    /// <returns>The ID of the inserted record.</returns>
    public int Insert<T>(string tableName, T entity, IDbTransaction? transaction = null)
    {
        EnsureInitialized();

        // Get properties excluding auto-increment columns
        var properties = typeof(T).GetProperties()
            .Where(p =>
            {
                var primaryKeyAttribute = p.GetCustomAttribute<PrimaryKeyAttribute>();
                return primaryKeyAttribute?.AutoIncrement != true;
            })
            .ToList();

        // Generate column and parameter lists
        var columns = string.Join(", ", properties.Select(p => p.Name));
        var parameters = string.Join(", ", properties.Select(p => $"@{p.Name}"));

        // Insert query
        var query = $"""
                     INSERT INTO {tableName} ({columns})
                     VALUES ({parameters});
                     """;

        // Execute the insert
        _dbContext!.Execute(query, entity, transaction);

        // Retrieve the last inserted ID using the dialect-specific query
        var lastInsertIdQuery = _sqlDialect!.GetLastInsertIdQuery(tableName);
        return _dbContext.ExecuteScalar<int>(lastInsertIdQuery, transaction);
    }

    /// <summary>
    ///     Updates an existing record in a specified table by its primary key within a transaction.
    /// </summary>
    /// <typeparam name="T">The type of the record to update.</typeparam>
    /// <param name="tableName">The name of the table to update.</param>
    /// <param name="idColumn">The name of the primary key column.</param>
    /// <param name="entity">The updated record.</param>
    /// <param name="transaction">
    ///     The database transaction to use when executing the command.
    ///     If null, the operation is executed without a transaction.
    /// </param>
    public void Update<T>(string tableName, string idColumn, T entity, IDbTransaction? transaction = null)
    {
        EnsureInitialized();
        var setClause = string.Join(", ", typeof(T).GetProperties()
            .Where(p => !p.Name.Equals(idColumn, StringComparison.OrdinalIgnoreCase))
            .Select(p => $"{p.Name} = @{p.Name}"));

        var query = $"""
                     UPDATE {tableName}
                     SET {setClause}
                     WHERE {idColumn} = @{idColumn}
                     """;

        _dbContext!.Execute(query, entity, transaction);
    }

    /// <summary>
    ///     Deletes a record from a specified table by its primary key within a transaction.
    /// </summary>
    /// <param name="tableName">The name of the table to delete from.</param>
    /// <param name="idColumn">The name of the primary key column.</param>
    /// <param name="idValue">The value of the primary key to delete.</param>
    /// <param name="transaction">
    ///     The database transaction to use when executing the command.
    ///     If null, the operation is executed without a transaction.
    /// </param>
    public void Delete(string tableName, string idColumn, object idValue, IDbTransaction? transaction = null)
    {
        EnsureInitialized();
        var query = $"DELETE FROM {tableName} WHERE {idColumn} = @Id";
        _dbContext!.Execute(query, new { Id = idValue }, transaction);
    }

    /// <summary>
    ///     Executes multiple database operations within a transaction.
    /// </summary>
    /// <param name="operations">A delegate containing the operations to execute within the transaction.</param>
    /// <exception cref="Exception">Re-throws any exceptions that occur within the transaction.</exception>
    public void ExecuteBatchTransaction(Action<IDbTransaction> operations)
    {
        EnsureInitialized();
        _dbContext!.ExecuteInTransaction(operations);
    }

    /// <summary>
    ///     Executes a custom query and returns the results as dynamic objects.
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">
    ///     The parameters to use with the SQL query. If null, no parameters are applied.
    /// </param>
    /// <returns>A list of dynamic objects representing the query results.</returns>
    public IEnumerable<dynamic> ExecuteDynamicQuery(string query, object? parameters = null)
    {
        EnsureInitialized();
        return _dbContext!.Query(query, parameters); // Use the dynamic-specific overload
    }

    /// <summary>
    ///     Ensures the OrmLite service has been properly initialized.
    /// </summary>
    private void EnsureInitialized()
    {
        if (_dbContext == null || _sqlDialect == null)
            throw new InvalidOperationException("OrmLite has not been initialized. Call Initialize() first.");
    }
}