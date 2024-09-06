using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace inmobiliaria_AT.Utils
{
    public static class PasswordUtils
    {
        // Hashea una contraseña usando un sal generado
        public static (string hashedPassword, string salt) HashPassword(string password)
        {
            // Generar un sal
            var saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            var salt = Convert.ToBase64String(saltBytes);

            // Hashear la contraseña con el sal
            var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return (hashedPassword, salt);
        }

        // Verifica una contraseña dada y su hash
        public static bool VerifyPassword(string password, string hashedPassword, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var hashToVerify = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashToVerify == hashedPassword;
        }
    }

}