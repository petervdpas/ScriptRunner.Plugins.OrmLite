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

var ormLite = new OrmLite();
ormLite.SetDbConnection(database);

// Register the User model
ormLite.RegisterModel<User>("Users");

// Insert a new user
var user = new User { Name = "Alice", Email = "alice@example.com" };
var userId = ormLite.Insert("Users", user, transaction: null);
Dump($"Inserted User ID: {userId}");

// Retrieve all users
var users = ormLite.GetAll<User>("Users");
DumpTable("All Users", users);

// Update a user
user.Name = "Alice Updated";
ormLite.Update("Users", "Id", user, transaction: null);

// Delete a user
ormLite.Delete("Users", "Id", userId);

database.CloseConnection();

return "OrmLite demo completed.";