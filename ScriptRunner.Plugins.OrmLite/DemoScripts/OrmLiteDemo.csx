/*
{
    "TaskCategory": "Plugins",
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

    public string? PhoneNumber { get; set; } // Optional field to showcase nullable property

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default value to showcase timestamps
}

// Initialize SqliteDatabase and OrmLite
var sqlDialect = new SQLiteDialect();
var database = new SqliteDatabase();
database.Setup("Data Source=enhanced_database.db");
database.OpenConnection();

var ormLite = new OrmLite();
ormLite.Initialize(database.GetConnection(), sqlDialect);

// Register the User model
ormLite.RegisterModel<User>();

// Insert multiple users
var usersToInsert = new List<User>
{
    new User { Name = "Alice", Email = "alice@example.com", PhoneNumber = "123-456-7890" },
    new User { Name = "Bob", Email = "bob@example.com" },
    new User { Name = "Charlie", Email = "charlie@example.com", PhoneNumber = "987-654-3210" }
};

foreach (var newUser in usersToInsert)
{
    var newUserId = ormLite.Insert("Users", newUser);
    Dump($"Inserted User ID: {newUserId}, Name: {newUser.Name}");
}

// Retrieve all users
var allUsers = ormLite.GetAll<User>("Users");
var allUsersDataTable = allUsers.ToDataTable();
DumpTable("All Users After Insertions", allUsersDataTable);

// Perform advanced filtering (search by name)
var searchName = "Alice";
var filteredUsers = allUsers.Where(u => u.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase)).ToList();
var filteredUsersTable = filteredUsers.ToDataTable();
DumpTable($"Filtered Users Containing '{searchName}'", filteredUsersTable);

// Update a user (add a phone number for Bob)
var userToUpdate = allUsers.FirstOrDefault(u => u.Name == "Bob");
if (userToUpdate != null)
{
    userToUpdate.PhoneNumber = "555-555-5555";
    ormLite.Update("Users", "Id", userToUpdate);
    Dump($"Updated User: {userToUpdate.Name}, Phone: {userToUpdate.PhoneNumber}");
}

// Retrieve and show updated user data
var updatedAllUsers = ormLite.GetAll<User>("Users");
var updatedUsersTable = updatedAllUsers.ToDataTable();
DumpTable("All Users After Updates", updatedUsersTable);

// Delete a specific user (Charlie)
var userToDelete = updatedAllUsers.FirstOrDefault(u => u.Name == "Charlie");
if (userToDelete != null)
{
    ormLite.Delete("Users", "Id", userToDelete.Id);
    Dump($"Deleted User ID: {userToDelete.Id}, Name: {userToDelete.Name}");
}

// Verify remaining users
var remainingUsers = ormLite.GetAll<User>("Users");
var remainingUsersDataTable = remainingUsers.ToDataTable();
DumpTable("Remaining Users", remainingUsersDataTable);

// Close database connection
database.CloseConnection();

return "Enhanced OrmLite demo completed.";
