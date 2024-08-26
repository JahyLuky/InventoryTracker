using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace InventoryTracker.Models
{
    /// <summary>
    /// Manages the SQLite database for the inventory system.
    /// </summary>
    public class InventoryDatabase
    {
        private SQLiteConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryDatabase"/> class.
        /// </summary>
        public InventoryDatabase()
        {
            InitializeDatabase();
        }

        /// <summary>
        /// Initializes the database by creating the necessary tables if they do not exist.
        /// </summary>
        private void InitializeDatabase()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InventoryDatabase.sqlite");
            _connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            _connection.Open();
            CreateInventoryTable();
            CreateUsersTable();
        }

        /// <summary>
        /// Creates the Users table in the database if it does not already exist.
        /// </summary>
        private void CreateUsersTable()
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS Users (
                UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL,
                Salt TEXT NOT NULL,
                Role TEXT NOT NULL
            );";

            ExecuteNonQuery(sql);
        }

        /// <summary>
        /// Creates the Inventory table in the database if it does not already exist.
        /// </summary>
        private void CreateInventoryTable()
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS Inventory (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Price REAL NOT NULL,
                    UserId INTEGER NOT NULL,
                    FOREIGN KEY(UserId) REFERENCES Users(UserId) ON DELETE CASCADE
                );";

            ExecuteNonQuery(sql);
        }


        /// <summary>
        /// Adds a new inventory item to the database.
        /// </summary>
        /// <param name="newItem">The new inventory item to add.</param>
        public void AddItem(InventoryItem newItem, long userId)
        {
            string sql = "INSERT INTO Inventory (Name, Quantity, Price, UserId) VALUES (@Name, @Quantity, @Price, @UserId); SELECT last_insert_rowid();";

            using (SQLiteCommand cmd = new SQLiteCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("@Name", newItem.Name);
                cmd.Parameters.AddWithValue("@Quantity", newItem.Quantity);
                cmd.Parameters.AddWithValue("@Price", newItem.Price);
                cmd.Parameters.AddWithValue("@UserId", userId);

                int newId = Convert.ToInt32(cmd.ExecuteScalar());
                if (newId <= 0)
                {
                    Debug.WriteLine($"\n\n!!!!!!!!!!newID = {newId}\n\n");
                }
                newItem.Id = newId;
            }
        }


        /// <summary>
        /// Retrieves an inventory item from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the inventory item to retrieve.</param>
        /// <returns>The inventory item with the specified ID, or null if not found.</returns>
        public InventoryItem GetItem(int id)
        {
            string sql = "SELECT Id, Name, Quantity, Price FROM Inventory WHERE Id = @Id;";

            using (SQLiteCommand cmd = new SQLiteCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        InventoryItem item = new InventoryItem
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = Convert.ToString(reader["Name"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            Price = Convert.ToDouble(reader["Price"])
                        };

                        return item;
                    }
                }
            }

            throw new Exception();
        }

        /// <summary>
        /// Updates an existing inventory item in the database.
        /// </summary>
        /// <param name="updatedItem">The updated inventory item.</param>
        public void UpdateItem(InventoryItem updatedItem, long userId)
        {
            string sql = "UPDATE Inventory SET Name = @Name, Quantity = @Quantity, Price = @Price WHERE Id = @Id AND UserId = @UserId;";

            using (SQLiteCommand cmd = new SQLiteCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("@Name", updatedItem.Name);
                cmd.Parameters.AddWithValue("@Quantity", updatedItem.Quantity);
                cmd.Parameters.AddWithValue("@Price", updatedItem.Price);
                cmd.Parameters.AddWithValue("@Id", updatedItem.Id);
                cmd.Parameters.AddWithValue("@UserId", userId);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes an inventory item from the database by its ID.
        /// </summary>
        /// <param name="itemId">The ID of the inventory item to delete.</param>
        public void DeleteItem(int itemId, long userId)
        {
            string sql = "DELETE FROM Inventory WHERE Id = @Id AND UserId = @UserId;";

            using (SQLiteCommand cmd = new SQLiteCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("@Id", itemId);
                cmd.Parameters.AddWithValue("@UserId", userId);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Retrieves all inventory items from the database.
        /// </summary>
        /// <returns>A list of all inventory items.</returns>
        public List<InventoryItem> GetAllItems(long userId)
        {
            List<InventoryItem> items = new List<InventoryItem>();

            string sql = "SELECT Id, Name, Quantity, Price FROM Inventory WHERE UserId = @UserId;";

            using (SQLiteCommand cmd = new SQLiteCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        InventoryItem item = new InventoryItem()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = Convert.ToString(reader["Name"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            Price = Convert.ToDouble(reader["Price"])
                        };

                        items.Add(item);
                    }
                }
            }

            return items;
        }


        /// <summary>
        /// Executes a non-query SQL command on the database.
        /// </summary>
        /// <param name="sql">The SQL command to execute.</param>
        private void ExecuteNonQuery(string sql)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _connection))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}
