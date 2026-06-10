using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TechStoreApp.Models;
using System.Linq;

namespace TechStoreApp.Services
{
    public static class AuthService
    {
        public static Customer? CurrentUser { get; private set; }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        public static bool Login(string email, string password)
        {
            using var db = new TechStoreDbContext();
            string hash = HashPassword(password);
            
            var user = db.Customers.FirstOrDefault(u => u.Email == email && u.PasswordHash == hash);
            if (user != null)
            {
                CurrentUser = user;
                return true;
            }
            return false;
        }

        public static bool Register(string email, string password, string firstName, string lastName, bool isAdmin = false)
        {
            using var db = new TechStoreDbContext();
            if (db.Customers.Any(c => c.Email == email)) return false;

            var customer = new Customer
            {
                Email = email,
                PasswordHash = HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = DateTime.Now,
                IsAdmin = isAdmin
            };

            db.Customers.Add(customer);
            db.SaveChanges();
            return true;
        }

        public static bool ChangePassword(string oldPassword, string newPassword)
        {
            if (CurrentUser == null) return false;

            using var db = new TechStoreDbContext();
            var user = db.Customers.Find(CurrentUser.CustomerId);
            if (user == null || user.PasswordHash != HashPassword(oldPassword)) return false;

            user.PasswordHash = HashPassword(newPassword);
            db.SaveChanges();
            CurrentUser = user;
            return true;
        }

        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}
