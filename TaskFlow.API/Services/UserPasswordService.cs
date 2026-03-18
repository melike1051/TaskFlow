using Microsoft.AspNetCore.Identity;
using TaskFlow.Core.Entities;

namespace TaskFlow.API.Services
{
    public class UserPasswordService
    {
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserPasswordService(IPasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(User user, string password, out bool shouldUpgradeHash)
        {
            shouldUpgradeHash = false;

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (verificationResult == PasswordVerificationResult.Success)
            {
                return true;
            }

            if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                shouldUpgradeHash = true;
                return true;
            }

            if (user.PasswordHash == password)
            {
                shouldUpgradeHash = true;
                return true;
            }

            return false;
        }
    }
}
