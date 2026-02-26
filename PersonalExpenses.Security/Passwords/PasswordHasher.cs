using PersonalExpenses.Security.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace PersonalExpenses.Security.Passwords
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password) =>
        BC.HashPassword(password);

        public bool VerifyPassword(string password, string hash) =>
        BC.Verify(password, hash);

    }
}
