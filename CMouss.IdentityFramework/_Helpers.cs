using CMouss.IdentityFramework.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Security.Claims;

namespace CMouss.IdentityFramework
{
    public static class Helpers
    {

        public static string GenerateId()
        {
            return GenerateId(IDFManager.IDGeneratorLevel.GetHashCode());
        }
        public static string GenerateId(int level)
        {
            int i = 0;
            string result = "";
            if (level < 1)
            {
                throw new Exception("level must be higher than 0");
            }
            while(i < level)
            {
                result = result + Guid.NewGuid().ToString().Replace(@"{", "").Replace(@"}", "").Replace(@"-","");
                i = i + 1;
            }

            return result;
        }

        public static string GenerateToken()
        {
            return CleanGUIDString(Guid.NewGuid().ToString() + Guid.NewGuid().ToString());
        }

        public static string GenerateKey()
        {
            return CleanGUIDString(Guid.NewGuid().ToString() + Guid.NewGuid().ToString() + Guid.NewGuid().ToString());
        }

        private static string CleanGUIDString(string guidString)
        {
            return guidString
                .Replace("-", "")
                .Replace("{", "")
                .Replace("}", "");
        }




        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string privateKey)
        {
            // Generate Salt and IV (Initialization Vector) - 128 bits each
            var saltStringBytes = Generate128BitsOfRandomEntropy();
            var ivStringBytes = Generate128BitsOfRandomEntropy();

            // Convert the plain text string to bytes
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Create a derived key from the private key, using the salt
            using (var password = new Rfc2898DeriveBytes(privateKey, saltStringBytes, 1000))  // 10000 iterations
            {
                var keyBytes = password.GetBytes(32);  // AES requires a 256-bit key (32 bytes)

                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;

                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                // Write all plain text bytes to the crypto stream for encryption
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();

                                // Get encrypted bytes from the memory stream
                                var cipherTextBytes = memoryStream.ToArray();

                                // Concatenate salt, IV, and the cipher text bytes
                                var finalBytes = new byte[saltStringBytes.Length + ivStringBytes.Length + cipherTextBytes.Length];
                                Buffer.BlockCopy(saltStringBytes, 0, finalBytes, 0, saltStringBytes.Length);
                                Buffer.BlockCopy(ivStringBytes, 0, finalBytes, saltStringBytes.Length, ivStringBytes.Length);
                                Buffer.BlockCopy(cipherTextBytes, 0, finalBytes, saltStringBytes.Length + ivStringBytes.Length, cipherTextBytes.Length);

                                // Convert the final concatenated byte array to a Base64 string
                                return Convert.ToBase64String(finalBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string encryptedText, string privateKey)
        {
            // Convert encrypted text (Base64) back to byte array
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(encryptedText);

            // Extract salt (first 16 bytes) and IV (next 16 bytes) from the byte array
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(16).ToArray();
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(16).Take(16).ToArray();
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(32).ToArray();

            // Derive key from the provided privateKey and extracted salt
            using (var password = new Rfc2898DeriveBytes(privateKey, saltStringBytes, 1000))
            {
                var keyBytes = password.GetBytes(32);  // AES-256 key

                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;

                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                // Read the entire encrypted data into a larger memory buffer
                                using (var resultStream = new MemoryStream())
                                {
                                    cryptoStream.CopyTo(resultStream);  // Copy decrypted bytes into the result stream
                                    var plainTextBytes = resultStream.ToArray();  // Get all decrypted bytes as array

                                    return Encoding.UTF8.GetString(plainTextBytes);  // Convert bytes back to string
                                }
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        private static byte[] Generate128BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16]; // 16 bytes = 128 bits
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        public static UserClaim GenerateUserClaim(User user)
        {
            UserClaim result = new();
            result.UserId = user.Id;
            result.UserName = user.UserName;
            result.UserFullName = user.FullName;
            result.EMail = user.Email;
            result.TokenCreateDate = DateTime.UtcNow;
            result.TokenExpireDate = DateTime.UtcNow.AddDays(IDFManager.TokenDefaultLifeTime.Days).AddHours(IDFManager.TokenDefaultLifeTime.Hours).AddMinutes(IDFManager.TokenDefaultLifeTime.Minutes);
            result.Roles = user.Roles.Select(o => o.Id).ToList();

            return result;
        }

        public static UserClaim DecryptUserToken(string encryptedToken)
        {
            UserClaim result = new();
            try
            {
                string str = Decrypt(encryptedToken, IDFManager.TokenEncryptionKey);
                result = JsonSerializer.Deserialize<UserClaim>(str);
                if (result.TokenExpireDate < DateTime.UtcNow) { throw new Exception ("Token has been Expired"); }
            }
            catch(Exception ex)
            {
                throw new Exception("Invalid Token");
            }

            return result;
        }


    }
}
