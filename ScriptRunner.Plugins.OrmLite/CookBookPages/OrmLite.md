---
Title: The OrmLite Database Plugin
Subtitle: Manage and Query Databases with OrmLite in ScriptRunner
Category: Cookbook
Author: Peter van de Pas
keywords: [CookBook, OrmLite, Database]
table-use-row-colors: true
table-row-color: "D3D3D3"
toc: true
toc-title: Table of Content
toc-own-page: true
---

# Recipe: Manage and Query Databases with OrmLite in ScriptRunner

## Goal

Learn how to use the **OrmLite** plugin in ScriptRunner to manage SQLite databases efficiently. 
This recipe covers creating, reading, updating, and deleting records, along with schema management and validation.

## Overview

This recipe demonstrates how to:
1. Define a database schema using attributes.
2. Perform CRUD operations (Create, Read, Update, Delete).
3. Dynamically query a database.
4. Validate data using attributes like **Required** and **Unique**.

By the end of this tutorial, you'll have a working script that interacts with an SQLite database using OrmLite.

---

## Steps

### 1. Define Task Metadata

Add metadata to the script for identification and categorization:

```csharp
/*
{
    "TaskCategory": "Database",
    "TaskName": "OrmLiteDemo",
    "TaskDetail": "A comprehensive demo showcasing OrmLite with SQLite."
}
*/
```

### 2. Define the Data Model

Use attributes to define the database schema and validation rules for the **User** model:

```csharp
[Table("Users")]
public class User
{
    [PrimaryKey(AutoIncrement = true)]
    public int Id { get; set; }

    [Required]
    [Unique]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    public string? PhoneNumber { get; set; } // Optional field

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default value
}
```

### 3. Initialize the Database and OrmLite

Set up the SQLite database and initialize the OrmLite plugin with the **SQLiteDialect**:

```csharp
var sqlDialect = new SQLiteDialect();
var database = new SqliteDatabase();
database.Setup("Data Source=example_database.db");
database.OpenConnection();

var ormLite = new OrmLite();
ormLite.Initialize(database.GetConnection(), sqlDialect);
```

### 4. Register the Model

Register the **User** model to create or ensure the existence of the corresponding table:

```csharp
ormLite.RegisterModel<User>();
```

### 5. Perform CRUD Operations

#### Insert Records
Add new users to the **Users** table:

```csharp
var user = new User { Name = "Alice", Email = "alice@example.com" };
var userId = ormLite.Insert("Users", user);
Dump($"Inserted User ID: {userId}");
```

#### Retrieve Records
Fetch all users from the database:

```csharp
var users = ormLite.GetAll<User>("Users");
DumpTable("All Users", users.ToDataTable());
```

#### Update Records
Update an existing user's details:

```csharp
user.Name = "Alice Updated";
ormLite.Update("Users", "Id", user);
Dump($"Updated User: {user.Name}");
```

#### Delete Records
Delete a user by their ID:

```csharp
ormLite.Delete("Users", "Id", userId);
Dump($"Deleted User ID: {userId}");
```

### 6. Validate Data

Validate data using attributes like **Required** and **Unique**:

```csharp
try
{
    ormLite.Validate(new User { Name = null, Email = "invalid@example.com" }); // Throws exception
}
catch (Exception ex)
{
    Dump($"Validation failed: {ex.Message}");
}
```

---

## Example Script

Here’s the complete script for reference:

```csharp
/*
{
    "TaskCategory": "Database",
    "TaskName": "OrmLiteDemo",
    "TaskDetail": "A comprehensive demo showcasing OrmLite with SQLite."
}
*/

[Table("Users")]
public class User
{
    [PrimaryKey(AutoIncrement = true)]
    public int Id { get; set; }

    [Required]
    [Unique]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

var sqlDialect = new SQLiteDialect();
var database = new SqliteDatabase();
database.Setup("Data Source=example_database.db");
database.OpenConnection();

var ormLite = new OrmLite();
ormLite.Initialize(database.GetConnection(), sqlDialect);
ormLite.RegisterModel<User>();

var user = new User { Name = "Alice", Email = "alice@example.com" };
var userId = ormLite.Insert("Users", user);
Dump($"Inserted User ID: {userId}");

var users = ormLite.GetAll<User>("Users");
DumpTable("All Users", users.ToDataTable());

user.Name = "Alice Updated";
ormLite.Update("Users", "Id", user);

ormLite.Delete("Users", "Id", userId);

database.CloseConnection();
return "OrmLite demo completed.";
```

---

## Expected Output

When executed, this script will:
1. Insert a user named Alice.
2. Retrieve and display all users.
3. Update Alice's name.
4. Delete Alice and verify the table is empty.

---

## Tips & Notes

- **Pluggable Dialects**: Extend OrmLite by creating custom SQL dialects.
- **Validation**: Leverage attributes like **Required** and **Unique** to ensure data integrity.
- **Transactions**: Use **ExecuteBatchTransaction** for complex workflows.
- **Dynamic Queries**: Utilize **ExecuteDynamicQuery** for flexible SQL operations.
