// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace NetCore.Test.PasswordHasher
{
    class Program
    {
        //Password => GUIDSalt, RNGSalt, PasswordHash
        static void Main(string[] args)
        {
            Console.Write("Enter a ID: ");
            string userId = Console.ReadLine();

            Console.Write("Enter a password: ");
            string password = Console.ReadLine();

            string guidSalt=Guid.NewGuid().ToString();
            string rngSalt = GetRNGSalt();
            string Passwordhashed = GetPasswordHash(userId, password, guidSalt, rngSalt);

            //데이터베이스의 비밀번호정보와 지금 입력한 비밀번호 정보를 비교해서 같은 해시값이 나오면 true, 아니면 false
            //로그인 성공
            bool check = CheckThePasswordInfo(userId, password, guidSalt, rngSalt, Passwordhashed);

            Console.WriteLine($"UserId: {userId}");
            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"GuidSalt: {guidSalt}");
            Console.WriteLine($"RNGSalt: {rngSalt}");
            Console.WriteLine($"Passwordhashed: {Passwordhashed}");
            Console.WriteLine($"Check: {(check ? "비밀번호 정보 일치" : "비밀번호 정보 불일치")}");
            Console.ReadLine();
        }
        private static string GetRNGSalt()
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        private static string GetPasswordHash(string userId, string password,string guidSalt, string rngSalt)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            // Pbkdf2
            // Password-based key derivation function 2
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                   password: userId + password + guidSalt,
                   salt: Encoding.UTF8.GetBytes(rngSalt),
                   prf: KeyDerivationPrf.HMACSHA512,
                   iterationCount: 45000, //10,000, 25,000, 45,000
                   numBytesRequested: 256 / 8));
        }

        private static bool CheckThePasswordInfo(string userId, string password, string guidSalt, string rngSalt, string Passwordhashed)
        {
            // Hash the incoming password and compare it to the hash
            // stored in the database.
            return GetPasswordHash(userId, password, guidSalt, rngSalt).Equals(Passwordhashed);
        }
    }
}


