using System.Security.Cryptography;
using System.Text;
using CoinDeskApi.Core.Interfaces;

namespace CoinDeskApi.Infrastructure.Services
{
    public class EncryptionService : IEncryptionService
    {
        public string EncryptAES(string plainText, string key)
        {
            using var aes = Aes.Create();
            aes.Key = GetKey(key, 32); // AES-256
            aes.IV = new byte[16]; // Zero IV for simplicity

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(encryptedBytes);
        }

        public string DecryptAES(string cipherText, string key)
        {
            using var aes = Aes.Create();
            aes.Key = GetKey(key, 32); // AES-256
            aes.IV = new byte[16]; // Zero IV for simplicity

            using var decryptor = aes.CreateDecryptor();
            var encryptedBytes = Convert.FromBase64String(cipherText);
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public (string publicKey, string privateKey) GenerateRSAKeyPair()
        {
            using var rsa = RSA.Create(2048);
            var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
            var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());

            return (publicKey, privateKey);
        }

        public string EncryptRSA(string plainText, string publicKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);

            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA256);

            return Convert.ToBase64String(encryptedBytes);
        }

        public string DecryptRSA(string cipherText, string privateKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);

            var encryptedBytes = Convert.FromBase64String(cipherText);
            var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        private static byte[] GetKey(string key, int length)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = keyBytes[i % keyBytes.Length];
            }

            return result;
        }
    }
}
