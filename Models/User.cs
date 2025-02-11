using System;
using System.Collections.Generic;

namespace CvBuilder.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } // Store hashed passwords
        public string PhoneNumber { get; set; }

        public ICollection<Resume> Resumes { get; set; } = new List<Resume>();

        public User(string fullName, string email, string passwordHash, string phoneNumber = "")
        {
            FullName = fullName;
            Email = email;
            PasswordHash = passwordHash;
            PhoneNumber = phoneNumber;
        }
    }
}