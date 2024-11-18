namespace InventoryTracker.Models
{
    public enum Roles
    {
        Admin,
        User,
        Visitor
    }

    public class User
    {
        public long UserId_ { get; set; }
        public string Username_ { get; set; }
        public Roles Role_ { get; set; }

        public User(long UserId, string UserName, Roles Role)
        {
            UserId_ = UserId;
            Username_ = UserName;
            Role_ = Role;
        }
    }
}
