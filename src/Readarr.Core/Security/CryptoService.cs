using System;
using System.Security.Cryptography;
using System.Text;
using Readarr.Core.Configuration;

namespace Readarr.Core.Security
{
    public interface ICryptoService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }

    public class CryptoService : ICryptoService
    {
        private readonly IConfigFileProvider _configFileProvider;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public CryptoService(IConfigFileProvider configFileProvider)
        {
            _configFileProvider = configFileProvider;
            
            // Generate or retrieve encryption key
            var instanceId = _configFileProvider.InstanceId;
            if (string.IsNullOrEmpty(instanceId))
            {
                instanceId = Guid.NewGuid().ToString();
                _configFileProvider.InstanceId = instanceId;
            }

            // Derive key and IV from instance ID
            using (var sha256 = SHA256.Create())
            {
                var keyMaterial = sha256.ComputeHash(Encoding.UTF8.GetBytes(instanceId + "readarr-key"));
                _key = new byte[32];
                Array.Copy(keyMaterial, _key, 32);

                var ivMaterial = sha256.ComputeHash(Encoding.UTF8.GetBytes(instanceId + "readarr-iv"));
                _iv = new byte[16];
                Array.Copy(ivMaterial, _iv, 16);
            }
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = _key;
                    aes.IV = _iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    var encryptor = aes.CreateEncryptor();
                    var plainBytes = Encoding.UTF8.GetBytes(plainText);
                    var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    
                    return Convert.ToBase64String(cipherBytes);
                }
            }
            catch (Exception)
            {
                // If encryption fails, return the plain text
                // This ensures backward compatibility
                return plainText;
            }
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                // Check if the text is already decrypted (backward compatibility)
                if (!IsBase64String(cipherText))
                    return cipherText;

                using (var aes = Aes.Create())
                {
                    aes.Key = _key;
                    aes.IV = _iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    var decryptor = aes.CreateDecryptor();
                    var cipherBytes = Convert.FromBase64String(cipherText);
                    var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
            catch (Exception)
            {
                // If decryption fails, assume it's not encrypted
                // This ensures backward compatibility
                return cipherText;
            }
        }

        private bool IsBase64String(string text)
        {
            if (string.IsNullOrEmpty(text) || text.Length % 4 != 0)
                return false;

            try
            {
                Convert.FromBase64String(text);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}