using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace CMouss.IdentityFramework.API.Serving
{
    public class TestController : Controller
    {

        [HttpPost]
        [Route(APIRoutes.Test.Echo)]
        public IActionResult Test()
        {
            return Ok("Echo test from CMouss.IdentityFramework, Client IPAddress: " + Request.HttpContext.Connection.RemoteIpAddress);
        }



        //        [HttpPost]
        //        [Route("api/Identity/test/secureEcho")]
        //        public IActionResult UserToken(
        //             [FromHeader] string userToken
        //        )
        //        {
        //            return Ok(CMouss.IdentityFramework.Helpers.Decrypt(userToken, IDFManager.TokenEncryptionKey));

        //        }

        //        [HttpPost]
        //        [Route("api/Identity/test/secureEcho1")]
        //        public IActionResult secureEcho1(
        //             [FromHeader] string userToken
        //        )
        //        {
        //            string x = "{\"UserId\":\"81e505f5ff6a497c908db6f1928d9722\",\"UserName\":\"Admin\",\"UserFullName\":\"Admin\",\"EMail\":\"Admin@mail.com\",\"TokenCreateDate\":\"2024-10-05T18:43:52.5662851Z\",\"TokenExpireDate\":\"2024-11-04T18:43:52.5663339Z\",\"Roles\":[\"Administrators\"]}";

        //            return Ok(Encrypt(x, IDFManager.TokenEncryptionKey));
        //        }

        //        [HttpPost]
        //        [Route("api/Identity/test/secureEcho2")]
        //        public IActionResult secureEcho2(
        //     [FromHeader] string userToken
        //)
        //        {
        //            string x = userToken;
        //            return Ok(Decrypt(x, "85947b8eb85e4538a89bf693736184c35b10c09fefcb4fa8b50f58a1afd169a548127574e79e4d43a59e54b6fb5b16f3"));
        //        }


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

        private static byte[] Generate128BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16]; // 16 bytes = 128 bits
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }




    }
}
