/*
{
    "TaskCategory": "Database",
    "TaskName": "OrmLiteDemo",
    "TaskDetail": "A demo script showcasing OrmLite with SQLite"
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
}

// Initialize SqliteDatabase and OrmLite
var sqlDialect = new SQLiteDialect();
var database = new SqliteDatabase();
database.Setup("Data Source=database.db");
database.OpenConnection();

var ormLite = new OrmLite();
ormLite.SetDbConnection(database.GetConnection());

// Register the User model
ormLite.RegisterModel<User>(sqlDialect);

// Insert a new user
var user = new User { Name = "Alice", Email = "alice@example.com" };
var userId = ormLite.Insert("Users", user, transaction: null);
Dump($"Inserted User ID: {userId}");

// Retrieve all users
var users = ormLite.GetAll<User>("Users");

// Convert to DataTable for DumpTable
var usersDataTable = users.ToDataTable();
DumpTable("All Users", usersDataTable);

// Update a user
user.Name = "Alice Updated";
ormLite.Update("Users", "Id", user, transaction: null);
Dump($"Updated User: {user.Name}");

// Retrieve and show updated user data
var updatedUsers = ormLite.GetAll<User>("Users");
var updatedUsersDataTable = updatedUsers.ToDataTable();
DumpTable("Updated Users", updatedUsersDataTable);

// Delete a user
ormLite.Delete("Users", "Id", userId);
Dump($"Deleted User ID: {userId}");

// Verify remaining users
var remainingUsers = ormLite.GetAll<User>("Users");
var remainingUsersDataTable = remainingUsers.ToDataTable();
DumpTable("Remaining Users", remainingUsersDataTable);

// Close database connection
database.CloseConnection();

return "OrmLite demo completed.";