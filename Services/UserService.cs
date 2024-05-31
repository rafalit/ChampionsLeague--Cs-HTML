using WebApplication1.Models;
using MongoDB.Driver;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace WebApplication1.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IClubsStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<User>("Users"); // Ta linijka definiuje kolekcję "Users"
        }

        public User Register(User user, string password)
        {
            user.PasswordHash = HashPassword(password);
            _users.InsertOne(user);
            return user;
        }

        public User Login(string username, string password)
        {
            var user = _users.Find(u => u.Username == username).FirstOrDefault();
            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }

        public List<User> Get()
        {
            return _users.Find(user=>true).ToList();
        }

        public User Get(string id)
        {
            return _users.Find(u => u.UserId == id).FirstOrDefault();
        }

        private string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var parts = hashedPassword.Split('.');
            var salt = Convert.FromBase64String(parts[0]);
            var hash = parts[1];

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hash == hashed;
        }
    }
}
