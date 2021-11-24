using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Passwork
{


    public static class CryptoUtils
    {

        private static SHA256 sha256 = SHA256Managed.Create();


        /// <summary>
        /// Creates a sha256 has of the input as a hex lowercase string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>A hex lowercase sha256 of the input.</returns>
        public static string Hash(string input)
        {
            var input_as_utf8 = Encoding.UTF8.GetBytes(input);
            var input_hash = sha256.ComputeHash(input_as_utf8);
            var hex = ByteArrayToString(input_hash).ToLower();
            return hex;
        }


        /// <summary>
        /// Generates a random string of charachters based on : A..Z, a..z and 0..9
        /// </summary>
        /// <param name="length">The length of the random string</param>
        /// <returns>A random string</returns>
        public static string GenerateString(int length)
        {
            var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                var index = RandomNumberGenerator.GetInt32(possible.Length);
                sb.Append(possible[index]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generates a random string of charachters based on : A..Z, a..z, 0..9 and !@#$%^
        /// </summary>
        /// <param name="length">The length of the random string</param>
        /// <returns>A random string</returns>
        public static string GeneratePassword(int length)
        {
            var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^";
            var sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                var index = System.Security.Cryptography.RandomNumberGenerator.GetInt32(possible.Length);
                sb.Append(possible[index]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Encodes the input based of the provided passphrase.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static string Encode(string input, string pass )
        {
            var crypt = CryptoJSEncrypt(input, pass);
            return Base32.ToBase32String(System.Text.Encoding.UTF8.GetBytes(crypt));
        }

        /// <summary>
        /// Decodes the input , by using a provided passphrase.
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static string Decode(string input, string pass)
        {
            var value = Base32.FromBase32String(input);
            return CryptoJSDecrypt(System.Text.Encoding.UTF8.GetString(value), pass);
        }


        public static async Task<string> EncryptString(string password, IVault vault, bool UseMasterPassword, string secret = null)
        {
            if (string.IsNullOrEmpty(password)) { return null; }
            if (UseMasterPassword)
            {
                var masterPass = string.IsNullOrEmpty(secret) ? await vault.GetMaster() : secret;
                return CryptoUtils.Encode(password, masterPass);
            }
            else
            {
                return Base64.Encode(password);
            }
        }

        public static async Task<string> DecryptString(string password, IVault vault, bool UseMasterPassword, string secret = null)
        {
            if (string.IsNullOrEmpty(password)) { return null; }
            if (UseMasterPassword)
            {
                var masterPass = string.IsNullOrEmpty(secret) ? await vault.GetMaster() : secret;
                return CryptoUtils.Decode(password, masterPass);
            }
            else
            {
                return Base64.Decode(password);
            }
        }

        /// <summary>
        /// CryptoJS compatible default modes AES encryption
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        private static string CryptoJSEncrypt(string plainText, string passphrase)
        {
            // generate salt
            byte[] key, iv;
            byte[] salt = new byte[8];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(salt);
            DeriveKeyAndIV(passphrase, salt, out key, out iv);
            // encrypt bytes
            byte[] encryptedBytes = EncryptStringToBytes_Aes(plainText, key, iv);
            // add salt as first 8 bytes
            byte[] encryptedBytesWithSalt = new byte[salt.Length + encryptedBytes.Length+8];
            Buffer.BlockCopy(Encoding.ASCII.GetBytes("Salted__"), 0, encryptedBytesWithSalt, 0, 8);
            Buffer.BlockCopy(salt, 0, encryptedBytesWithSalt, 8, salt.Length);
            Buffer.BlockCopy(encryptedBytes, 0, encryptedBytesWithSalt, salt.Length+8 , encryptedBytes.Length);
            // base64 encode
            return Convert.ToBase64String(encryptedBytesWithSalt);
        }

        /// <summary>
        /// CryptoJS compatible default modes AES decryption
        /// </summary>
        /// <param name="encrypted"></param>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        private static string CryptoJSDecrypt(string encrypted, string passphrase)
        {
            // base 64 decode
            byte[] encryptedBytesWithSalt = Convert.FromBase64String(encrypted);
            // extract salt (first 8 bytes of encrypted)
            byte[] salt = new byte[8];
            byte[] encryptedBytes = new byte[encryptedBytesWithSalt.Length - salt.Length - 8];
            Buffer.BlockCopy(encryptedBytesWithSalt, 8, salt, 0, salt.Length);
            Buffer.BlockCopy(encryptedBytesWithSalt, salt.Length + 8, encryptedBytes, 0, encryptedBytes.Length);
            // get key and iv
            byte[] key, iv;
            DeriveKeyAndIV(passphrase, salt, out key, out iv);
            return DecryptStringFromBytes_Aes(encryptedBytes, key, iv);
        }

        /// <summary>
        /// Derives the Key and IV based on the provided passphrase
        /// </summary>
        /// <param name="passphrase"></param>
        /// <param name="salt"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        private static void DeriveKeyAndIV(string passphrase, byte[] salt, out byte[] key, out byte[] iv)
        {
            // generate key and iv
            List<byte> concatenatedHashes = new List<byte>(48);

            byte[] password = Encoding.UTF8.GetBytes(passphrase);
            byte[] currentHash = new byte[0];

            var md5 = MD5.Create();
            bool enoughBytesForKey = false;
            // See http://www.openssl.org/docs/crypto/EVP_BytesToKey.html#KEY_DERIVATION_ALGORITHM
            while (!enoughBytesForKey)
            {
                int preHashLength = currentHash.Length + password.Length + salt.Length;
                byte[] preHash = new byte[preHashLength];

                Buffer.BlockCopy(currentHash, 0, preHash, 0, currentHash.Length);
                Buffer.BlockCopy(password, 0, preHash, currentHash.Length, password.Length);
                Buffer.BlockCopy(salt, 0, preHash, currentHash.Length + password.Length, salt.Length);

                currentHash = md5.ComputeHash(preHash);
                concatenatedHashes.AddRange(currentHash);

                if (concatenatedHashes.Count >= 48)
                    enoughBytesForKey = true;
            }

            key = new byte[32];
            iv = new byte[16];
            concatenatedHashes.CopyTo(0, key, 0, 32);
            concatenatedHashes.CopyTo(32, iv, 0, 16);

            md5.Clear();
            md5 = null;
        }


       
        private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an AesManaged object
            // with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an AesManaged object
            // with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        { 
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        private static string ByteArrayToString(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
