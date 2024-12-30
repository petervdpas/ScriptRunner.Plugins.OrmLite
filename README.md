# ScriptRunner.Plugins.OrmLite

![License](https://img.shields.io/badge/license-MIT-green)  
![Version](https://img.shields.io/badge/version-1.0.0-blue)

`OrmLite` is a powerful and flexible plugin for **ScriptRunner**, providing seamless database management across 
multiple SQL dialects. This plugin supports CRUD operations, schema management, dynamic queries, 
and transactional workflows with a focus on simplicity and extensibility.

---

## 🚀 Features

- **Cross-Dialect Support**: Works with SQLite, MySQL, PostgreSQL, and SQL Server via pluggable SQL dialects.
- **CRUD Operations**: Effortlessly create, read, update, and delete records.
- **Dynamic Query Execution**: Execute custom SQL queries with dynamic parameterization.
- **Schema Management**: Automatically create or update tables based on annotated model definitions.
- **Attribute-Based Validation**: Enforce rules like `Required` and `Unique` directly in your model classes.
- **Transactional Workflows**: Perform multiple operations atomically.
- **Seamless SQLite Integration**: Includes a lightweight SQLite dialect for quick setups.

---

## 📦 Installation

### Plugin Activation
1. Add the `ScriptRunner.Plugins.OrmLite` assembly to your ScriptRunner project's `Plugins` folder.
2. Include required dependencies like `Microsoft.Data.Sqlite` for SQLite support.
3. The plugin will be automatically detected and available for use.

---

## 📖 Usage

### Getting Started

This example demonstrates a fully functional script using `OrmLite` to manage a simple `Users` table:

```csharp
/*
{
    "TaskCategory": "Database",
    "TaskName": "OrmLiteDemo",
    "TaskDetail": "A comprehensive demo script showcasing OrmLite with SQLite"
}
*/

// Define a User model
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
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default timestamp
}

// Initialize database and OrmLite
var sqlDialect = new SQLiteDialect();
var database = new SqliteDatabase();
database.Setup("Data Source=example_database.db");
database.OpenConnection();

var ormLite = new OrmLite();
ormLite.Initialize(database.GetConnection(), sqlDialect);

// Register the User model
ormLite.RegisterModel<User>();

// Insert users
var user = new User { Name = "Alice", Email = "alice@example.com" };
var userId = ormLite.Insert("Users", user);
Dump($"Inserted User ID: {userId}");

// Retrieve and display all users
var users = ormLite.GetAll<User>("Users");
DumpTable("All Users", users);

// Update a user
user.Name = "Alice Updated";
ormLite.Update("Users", "Id", user);

// Delete the user
ormLite.Delete("Users", "Id", userId);

database.CloseConnection();

return "OrmLite demo completed.";
```

---

## 🔧 Configuration

### Setting Up the Database
```csharp
var database = new SqliteDatabase();
database.Setup("Data Source=example.db");
database.OpenConnection();
```

### Registering Models
Use attributes to define table schema:
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
}
```
Register the model:
```csharp
ormLite.RegisterModel<User>();
```

---

## 🌟 Advanced Features

### Dynamic Queries
```csharp
var results = ormLite.ExecuteDynamicQuery("SELECT * FROM Users WHERE Name = @Name", new { Name = "Alice" });
DumpTable("Dynamic Query Results", results);
```

### Transactions
```csharp
ormLite.ExecuteBatchTransaction(transaction =>
{
    ormLite.Insert("Users", new User { Name = "Jane" }, transaction);
    ormLite.Update("Users", "Id", new User { Id = 1, Name = "Updated Jane" }, transaction);
});
```

### Validation
```csharp
ormLite.Validate(new User { Name = null, Email = "invalid@example.com" }); // Throws exception if invalid
```

---

## 🧪 Testing

Test scenarios include:
- CRUD operations with real-world data.
- Attribute-based validation edge cases.
- Transactional workflows for atomic operations.

---

## 🛠 Extending OrmLite

OrmLite supports pluggable SQL dialects. Add support for a new database by implementing `ISqlDialect`.

Example: Custom Dialect
```csharp
public class CustomDialect : ISqlDialect
{
    public string MapType(Type type) { /* Custom mapping logic */ }
    public string PrimaryKeySyntax => "CUSTOM PRIMARY KEY";
    public string AutoIncrementSyntax => "CUSTOM AUTOINCREMENT";
    public string UniqueSyntax => "CUSTOM UNIQUE";
    public string NotNullSyntax => "CUSTOM NOT NULL";
    public string GetLastInsertIdQuery(string tableName) => "SELECT LAST_ID()";
}
```

---

## 📄 Contributing

Contributions are welcome! Follow these steps:
1. Fork the repository.
2. Create a feature branch.
3. Submit a pull request.

---

## 🔗 Links

- [ScriptRunner Plugins Repository](https://github.com/petervdpas/ScriptRunner.Plugins)

---

## License

Licensed under the [MIT License](./LICENSE).
