namespace InventoryTracker.Models
{
    /// <summary>
    /// Represents an application user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is an administrator.
        /// </summary>
        public bool IsAdmin { get; set; }  // New property to indicate admin status

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
        {
            // Default constructor
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class with specified values.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="username">The username.</param>
        /// <param name="isAdmin">A value indicating whether the user is an administrator.</param>
        public User(long userId, string username, bool isAdmin)
        {
            UserId = userId;
            Username = username;
            IsAdmin = isAdmin;
        }
    }
}
