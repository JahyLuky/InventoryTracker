using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace InventoryTracker.Models
{
    /// <summary>
    /// Provides user management services such as creating users, logging in, and logging out.
    /// </summary>
    public class UserService
    {
        private readonly string _dbConnectionString;
        private PasswordHasher _passwordHasher;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// Sets up the database connection string, initializes the database, and adds hardcoded users.
        /// </summary>
        public UserService()
        {
            _dbConnectionString = $"Data Source={GetDatabasePath()};Version=3;";
            InitializeDatabase();
            _passwordHasher = new PasswordHasher();
            InitializeAdmin(); // Initialize hardcoded admin
        }

        /// <summary>
        /// Gets the path to the SQLite database file.
        /// </summary>
        /// <returns>The database file path.</returns>
        private string GetDatabasePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InventoryDatabase.sqlite");
        }

        /// <summary>
        /// Initializes the database by creating necessary tables if they do not exist.
        /// </summary>
        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(_dbConnectionString))
            {
                connection.Open();

                CreateUsersTable(connection);
                CreateSessionsTable(connection);
                CreateInventoryTable(connection);
            }
        }

        /// <summary>
        /// Creates the Users table in the database.
        /// </summary>
        /// <param name="connection">The SQLite connection.</param>
        private void CreateUsersTable(SQLiteConnection connection)
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS Users (
                    UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    Salt TEXT NOT NULL,
                    Role TEXT NOT NULL
                );";

            ExecuteNonQuery(connection, sql);
        }

        /// <summary>
        /// Creates the Sessions table in the database.
        /// </summary>
        /// <param name="connection">The SQLite connection.</param>
        private void CreateSessionsTable(SQLiteConnection connection)
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS Sessions (
                    SessionId INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    LoginTime TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    LogoutTime TIMESTAMP,
                    FOREIGN KEY (UserId) REFERENCES Users(UserId)
                );";

            ExecuteNonQuery(connection, sql);
        }

        /// <summary>
        /// Creates the Inventory table in the database.
        /// </summary>
        /// <param name="connection">The SQLite connection.</param>
        private void CreateInventoryTable(SQLiteConnection connection)
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS Inventory (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Price REAL NOT NULL
                );";

            ExecuteNonQuery(connection, sql);
        }

        /// <summary>
        /// Initializes hardcoded admin in the database.
        /// </summary>
        private void InitializeAdmin()
        {
            using (var connection = new SQLiteConnection(_dbConnectionString))
            {
                connection.Open();
                if (!UsernameExists(connection, "admin"))
                {
                    string salt;
                    string hashedPassword = _passwordHasher.HashPassword("admin", out salt);
                    CreateUser(connection, "admin", hashedPassword, salt, "Admin");
                }
            }
        }

        /// <summary>
        /// Creates a new user with the specified username, password, and role.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="role">The role.</param>
        /// <returns>True if the user was created successfully, otherwise false.</returns>
        public bool CreateUser(string username, string password, string role)
        {
            string salt;
            string hashedPassword = _passwordHasher.HashPassword(password, out salt);

            using (var connection = new SQLiteConnection(_dbConnectionString))
            {
                connection.Open();
                if (UsernameExists(connection, username))
                {
                    return false;
                }
                return CreateUser(connection, username, hashedPassword, salt, role);
            }
        }

        /// <summary>
        /// Creates a new user with the specified details in the database.
        /// </summary>
        /// <param name="connection">The SQLite connection.</param>
        /// <param name="username">The username.</param>
        /// <param name="passwordHash">The hashed password.</param>
        /// <param name="salt">The salt used for hashing the password.</param>
        /// <param name="role">The role.</param>
        /// <returns>True if the user was created successfully, otherwise false.</returns>
        private bool CreateUser(SQLiteConnection connection, string username, string passwordHash, string salt, string role)
        {
            string sql = @"
                INSERT INTO Users (Username, PasswordHash, Salt, Role)
                VALUES (@Username, @PasswordHash, @Salt, @Role);";

            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                cmd.Parameters.AddWithValue("@Salt", salt);
                cmd.Parameters.AddWithValue("@Role", role);

                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine($"Error creating user: {ex.Message}");
                    return false;
                }
            }
        }

        public long GetUserID(string username)
        {
            string sql = "SELECT UserId FROM Users WHERE Username = @Username;";
            using (var connection = new SQLiteConnection(_dbConnectionString))
            {
                connection.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            object userIdObj = reader["UserId"];
                            if (userIdObj != DBNull.Value)
                            {
                                return Convert.ToInt64(userIdObj);
                            }
                        }
                    }
                }
            }
            return 0;
        }


        /// <summary>
        /// Attempts to log in a user with the specified username and password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="isAdmin">Outputs whether the user has admin privileges.</param>
        /// <returns>True if login was successful, otherwise false.</returns>
        public bool Login(string username, string password, out bool isAdmin)
        {
            string sql = "SELECT * FROM Users WHERE Username = @Username;";

            using (var connection = new SQLiteConnection(_dbConnectionString))
            {
                connection.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedHash = reader["PasswordHash"].ToString();
                            string storedSalt = reader["Salt"].ToString();

                            // Verify password
                            if (_passwordHasher.VerifyPassword(password, storedHash, storedSalt))
                            {
                                // Successful login, log session
                                CurrentUserId = Convert.ToInt64(reader["UserId"]);
                                LogSession(connection, CurrentUserId);
                                isAdmin = IsAdminRole(reader["Role"].ToString());
                                return true;
                            }
                        }
                    }
                }
            }

            isAdmin = false;
            return false;
        }

        /// <summary>
        /// Logs out the current user by updating the logout time of the latest session.
        /// </summary>
        public void Logout()
        {
            // Update the latest session for the current user with LogoutTime
            string sql = "UPDATE Sessions SET LogoutTime = CURRENT_TIMESTAMP WHERE UserId = @UserId AND LogoutTime IS NULL;";

            using (var connection = new SQLiteConnection(_dbConnectionString))
            {
                connection.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Retrieves all usernames from the database.
        /// </summary>
        /// <returns>A list of usernames.</returns>
        public List<string> GetAllUsernames()
        {
            List<string> usernames = new List<string>();

            string sql = "SELECT Username FROM Users;";

            using (var connection = new SQLiteConnection(_dbConnectionString))
            {
                connection.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string username = reader["Username"].ToString();
                            usernames.Add(username);
                        }
                    }
                }
            }

            return usernames;
        }

        /// <summary>
        /// Checks if a username exists in the database.
        /// </summary>
        /// <param name="connection">The SQLite connection.</param>
        /// <param name="username">The username to check.</param>
        /// <returns>True if the username exists, otherwise false.</returns>
        private bool UsernameExists(SQLiteConnection connection, string username)
        {
            string sql = "SELECT COUNT(*) FROM Users WHERE Username = @Username;";

            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                long count = (long)cmd.ExecuteScalar();
                Debug.WriteLine($"!!!!!!!!!! Username: {username}, Count: {count}");
                return count > 0;
            }
        }

        /// <summary>
        /// Logs a new session for the specified user.
        /// </summary>
        /// <param name="connection">The SQLite connection.</param>
        /// <param name="userId">The user ID.</param>
        private void LogSession(SQLiteConnection connection, long userId)
        {
            // Insert new session record
            string sql = "INSERT INTO Sessions (UserId) VALUES (@UserId);";

            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes a non-query SQL command.
        /// </summary>
        /// <param name="connection">The SQLite connection.</param>
        /// <param name="sql">The SQL command.</param>
        private void ExecuteNonQuery(SQLiteConnection connection, string sql)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Checks if the user with the specified username has admin privileges.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>True if the user is an admin, otherwise false.</returns>
        public bool IsAdmin(string username)
        {
            string sql = "SELECT Role FROM Users WHERE Username = @Username;";

            using (var connection = new SQLiteConnection(_dbConnectionString))
            {
                connection.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    string role = cmd.ExecuteScalar()?.ToString();
                    return IsAdminRole(role);
                }
            }
        }

        /// <summary>
        /// Determines if the specified role is an admin role.
        /// </summary>
        /// <param name="role">The role to check.</param>
        /// <returns>True if the role is admin, otherwise false.</returns>
        private bool IsAdminRole(string role)
        {
            return role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets or sets the current user's ID.
        /// </summary>
        public long CurrentUserId { get; set; }
    }
}
