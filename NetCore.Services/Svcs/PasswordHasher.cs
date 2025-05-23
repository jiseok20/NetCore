﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;

namespace NetCore.Services.Svcs
{
    public class PasswordHasher : IPasswordHasher
    {
        private DBFirstDbContext _context;
        public PasswordHasher(DBFirstDbContext context) 
        {
            _context = context;
        }
        #region Private methods
        private string GetGUIDSalt() 
        {
            return Guid.NewGuid().ToString();
        }
        private string GetRNGSalt()
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        private string GetPasswordHash(string userId, string password, string guidSalt, string rngSalt)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            // Pbkdf2
            // Password-based key derivation function 2
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                   password: userId + password + guidSalt,
                   salt: Encoding.UTF8.GetBytes(rngSalt),
                   prf: KeyDerivationPrf.HMACSHA512,
                   iterationCount: 45000, //10000, 25000, 45000
                   numBytesRequested: 256 / 8));
        }

        private bool CheckThePasswordInfo(string userId, string password, string guidSalt, string rngSalt, string Passwordhashed)
        {
            // Hash the incoming password and compare it to the hash
            // stored in the database.
            return GetPasswordHash(userId, password, guidSalt, rngSalt).Equals(Passwordhashed);
        }
        #endregion

        string IPasswordHasher.GetGUIDSalt()
        {
            return GetGUIDSalt();
        }

        string IPasswordHasher.GetRNGSalt()
        {
            return GetRNGSalt();
        }

        string IPasswordHasher.GetPasswordHash(string userId, string password, string guidSalt, string rngSalt)
        {
            return GetPasswordHash(userId, password, guidSalt, rngSalt);
        }

        bool IPasswordHasher.MatchTheUserInfo(string userId, string password)
        {
            var user = _context.Users.Where(u=>u.UserId.Equals(userId)).FirstOrDefault();

            string guidSalt = user.GUIDSalt;
            string rngSalt = user.RNGSalt;
            string Passwordhashed=user.PasswordHash;

            return CheckThePasswordInfo(userId,password, guidSalt, rngSalt, Passwordhashed);
        }
        
    }
}
