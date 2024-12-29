# ScriptRunner.Plugins.OrmLite

![License](https://img.shields.io/badge/license-MIT-green)  
![Version](https://img.shields.io/badge/version-1.0.0-blue)

`OrmLite` is a lightweight and powerful plugin for **ScriptRunner**, offering seamless database management and operations. With support for SQLite via `SqliteDatabase`, it provides CRUD operations, schema management, dynamic queries, and transactional support.

---

## 🚀 Features

- **CRUD Operations**: Create, read, update, and delete records in your database.
- **Dynamic Query Execution**: Run custom SQL queries with dynamic parameterization.
- **Schema Management**: Automatically create or update tables based on model definitions.
- **Attribute-Based Validation**: Enforce validation rules on your models with attributes like `Required` and `Unique`.
- **Transactional Support**: Perform multiple operations in a single transaction for atomicity.
- **Seamless SQLite Integration**: Works with `SqliteDatabase` for managing SQLite connections and queries.

---

## 📦 Installation

### Plugin Activation
1. Place the `ScriptRunner.Plugins.OrmLite` assembly in the `Plugins` folder of your ScriptRunner project.
2. Ensure the required dependencies (`Microsoft.Data.Sqlite`) are available.
3. The plugin will be automatically discovered and activated.

---

## 📖 Usage

### Writing a Script

Here’s an example script showcasing the use of OrmLite with a user database:

```csharp
/*
{
    "TaskCategory": "Database",
    "TaskName": "OrmLiteDemo",
    "TaskDetail": "A demo script showcasing OrmLite with SQLite"
}
*/

// Define a User model
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

// Initialize SqliteDatabase and OrmLite
var database = new SqliteDatabase();
database.Setup("Data Source=database.db");
database.OpenConnection();

var OrmLite = new OrmLite();
OrmLite.SetDbConnection(database);

// Register the User model
OrmLite.RegisterModel<User>("Users");

// Insert a new user
var user = new User { Name = "Alice", Email = "alice@example.com" };
var userId = OrmLite.Insert("Users", user, transaction: null);
Dump($"Inserted User ID: {userId}");

// Retrieve all users
var users = OrmLite.GetAll<User>("Users");
DumpTable("All Users", users);

// Update a user
user.Name = "Alice Updated";
OrmLite.Update("Users", "Id", user, transaction: null);

// Delete a user
OrmLite.Delete("Users", "Id", userId);

database.CloseConnection();

return "OrmLite demo completed.";
```

---

## 🔧 Configuration

### Setting Up the Database
Initialize the SQLite database connection using `SqliteDatabase`:
```csharp
var database = new SqliteDatabase();
database.Setup("Data Source=database.db");
database.OpenConnection();
```

### Registering Models
Define your models and register them with table names:
```csharp
OrmLite.RegisterModel<User>("Users");
```

### CRUD Operations
Perform Create, Read, Update, and Delete operations:
```csharp
var user = new User { Name = "John", Email = "john@example.com" };

// Insert
var userId = OrmLite.Insert("Users", user, transaction: null);

// Read
var users = OrmLite.GetAll<User>("Users");

// Update
user.Name = "John Updated";
OrmLite.Update("Users", "Id", user);

// Delete
OrmLite.Delete("Users", "Id", userId);
```

---

## 🌟 Advanced Features

### Dynamic Queries
Run custom SQL queries dynamically:
```csharp
var results = OrmLite.ExecuteDynamicQuery("SELECT * FROM Users WHERE Name = @Name", new { Name = "Alice" });
DumpTable("Dynamic Query Results", results);
```

### Transactions
Perform multiple operations in a single transaction:
```csharp
OrmLite.ExecuteBatchTransaction(transaction =>
{
    OrmLite.Insert("Users", new User { Name = "Jane" }, transaction);
    OrmLite.Update("Users", "Id", new User { Id = 1, Name = "Updated Jane" }, transaction);
});
```

### Validation
Ensure entities meet defined validation rules:
```csharp
OrmLite.Validate(new User { Name = null, Email = "invalid@example.com" }); // Throws exception if invalid
```

---

## 🧪 Testing

- Test CRUD operations with sample models and SQLite databases.
- Verify transactional behavior with complex workflows.
- Test attribute-based validation for edge cases.

---

## 📄 Contributing

1. Fork this repository.
2. Create a feature branch (`git checkout -b feature/YourFeature`).
3. Commit your changes (`git commit -m 'Add YourFeature'`).
4. Push the branch (`git push origin feature/YourFeature`).
5. Open a pull request.

---

## Author

Developed with ❤️ by **Peter van de Pas**.

For questions, feedback, or contributions, feel free to open an issue or contact me directly!

---

## 🔗 Links

- [ScriptRunner Plugins Repository](https://github.com/petervdpas/ScriptRunner.Plugins)

---

## License

This project is licensed under the [MIT License](./LICENSE).

