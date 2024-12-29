using System;
using System.Collections.Generic;
using System.Data;

namespace ScriptRunner.Plugins.OrmLite.Interfaces;

/// <summary>
/// Defines a generic interface for a data service that provides CRUD operations, 
/// dynamic query execution, and schema management.
/// </summary>
public interface IOrmLite
{
    /// <summary>
    /// Sets the database connection to be used by the service.
    /// </summary>
    /// <param name="dbConnection">The database connection to use.</param>
    void SetDbConnection(IDbConnection dbConnection);

    /// <summary>
    /// Registers a model type and ensures the database schema matches the model.
    /// </summary>
    /// <typeparam name="T">The type of the model to register.</typeparam>
    /// <param name="tableName">The name of the table for the model.</param>
    void RegisterModel<T>(string tableName);

    /// <summary>
    /// Validates an entity's properties based on specified attributes.
    /// </summary>
    /// <typeparam name="T">The type of the entity to validate.</typeparam>
    /// <param name="entity">The entity to validate.</param>
    /// <exception cref="InvalidOperationException">Thrown if validation fails.</exception>
    void Validate<T>(T entity);

    /// <summary>
    /// Retrieves all records of a given type from a specified table.
    /// </summary>
    /// <typeparam name="T">The type of the records to retrieve.</typeparam>
    /// <param name="tableName">The name of the table to query.</param>
    /// <returns>A collection of records of type <typeparamref name="T"/>.</returns>
    IEnumerable<T> GetAll<T>(string tableName);

    /// <summary>
    /// Retrieves a record by its primary key from a specified table.
    /// </summary>
    /// <typeparam name="T">The type of the record to retrieve.</typeparam>
    /// <param name="tableName">The name of the table to query.</param>
    /// <param name="idColumn">The name of the primary key column.</param>
    /// <param name="idValue">The value of the primary key to search for.</param>
    /// <returns>The matching record of type <typeparamref name="T"/>, or null if not found.</returns>
    T? GetById<T>(string tableName, string idColumn, object idValue);

    /// <summary>
    /// Inserts a new record into a specified table and returns the generated ID.
    /// </summary>
    /// <typeparam name="T">The type of the record to insert.</typeparam>
    /// <param name="tableName">The name of the table to insert into.</param>
    /// <param name="entity">The record to insert.</param>
    /// <param name="transaction">The transaction within which to execute the command.</param>
    /// <returns>The ID of the inserted record.</returns>
    int Insert<T>(string tableName, T entity, IDbTransaction transaction);

    /// <summary>
    /// Updates an existing record in a specified table by its primary key.
    /// </summary>
    /// <typeparam name="T">The type of the record to update.</typeparam>
    /// <param name="tableName">The name of the table to update.</param>
    /// <param name="idColumn">The name of the primary key column.</param>
    /// <param name="entity">The updated record.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    void Update<T>(string tableName, string idColumn, T entity, IDbTransaction? transaction = null);

    /// <summary>
    /// Deletes a record from a specified table by its primary key.
    /// </summary>
    /// <param name="tableName">The name of the table to delete from.</param>
    /// <param name="idColumn">The name of the primary key column.</param>
    /// <param name="idValue">The value of the primary key to delete.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    void Delete(string tableName, string idColumn, object idValue, IDbTransaction? transaction = null);

    /// <summary>
    /// Executes multiple database operations within a transaction.
    /// </summary>
    /// <param name="operations">A delegate containing the operations to execute within the transaction.</param>
    /// <exception cref="Exception">Re-throws any exceptions that occur within the transaction.</exception>
    void ExecuteBatchTransaction(Action<IDbTransaction> operations);

    /// <summary>
    /// Executes a custom query and returns the results as dynamic objects.
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">The parameters for the query, if any.</param>
    /// <returns>A collection of dynamic objects representing the query results.</returns>
    IEnumerable<dynamic> ExecuteDynamicQuery(string query, object? parameters = null);
}