using System;
using System.Linq;
using CvBuilder.Data;
using CvBuilder.Models;
using BCrypt.Net;

namespace CvBuilder.Services
{
    public class UserCreation
    {
        private readonly CvBuilderContext _db;

        public UserCreation(CvBuilderContext db)
        {
            _db = db;
        }

        public void RegisterUser()
        {
            Console.Write("Enter full name: ");
            string fullName = Console.ReadLine()!;
            Console.Write("Enter email: ");
            string email = Console.ReadLine()!;
            Console.Write("Enter password: ");
            string password = Console.ReadLine()!;
            Console.Write("Enter phone number (optional): ");
            string? phoneNumber = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("\nAll fields except phone number are required!");
                return;
            }

            // Check if email already exists
            if (_db.Users.Any(u => u.Email == email))
            {
                Console.WriteLine("\nEmail already registered.");
                return;
            }

            // Hash password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            User newUser = new User(fullName, email, hashedPassword, phoneNumber ?? "");

            _db.Users.Add(newUser);
            _db.SaveChanges();

            Console.WriteLine("\nUser registered successfully!");
        }

        public User? LoginUser(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("\nInvalid email or password.");
                return null;
            }

            var user = _db.Users.SingleOrDefault(u => u.Email == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                Console.WriteLine("\nInvalid email or password.");
                return null;
            }

            Console.WriteLine("\nLogin successful!");
            return user;
        }
    }
}