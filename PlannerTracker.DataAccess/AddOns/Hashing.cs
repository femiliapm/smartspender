namespace PlannerTracker.DataAccess.AddOns
{
    using BCrypt.Net;

    public class Hashing
    {
        public string HashPassword(string rawPassword)
        {
            return BCrypt.EnhancedHashPassword(rawPassword, HashType.SHA256);
        }

        public bool ValidatePassword(string rawPassword, string hashPassword)
        {
            return BCrypt.EnhancedVerify(rawPassword, hashPassword, HashType.SHA256);
        }
    }
}
