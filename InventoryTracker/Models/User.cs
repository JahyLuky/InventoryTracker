namespace InventoryTracker.Models
{
    public class User
    {
        public long UserId_ { get; set; }
        public string Username_ { get; set; }
        public string Role_ { get; set; }

        public User(long UserId, string UserName, string Role)
        {
            UserId_ = UserId;
            Username_ = UserName;
            Role_ = Role;
        }
    }
}
