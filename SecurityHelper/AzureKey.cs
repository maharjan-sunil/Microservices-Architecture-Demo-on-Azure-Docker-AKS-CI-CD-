using Azure.Identity;
using Azure.Security.KeyVault.Keys.Cryptography;
using System.Text;

namespace UserService.API.Security
{
    public class AzureKey
    {
        private readonly CryptographyClient _cryptoClient;

        public AzureKey()
        {
            string keyUri = "https://dockerk8s.vault.azure.net/keys/DbConnectionString/2611aab75db248c8a35191ee44249dfc";
            _cryptoClient = new CryptographyClient(new Uri(keyUri), new DefaultAzureCredential());
        }

        public async Task<string> Encrypt(string plainText)
        {
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);

            // Use the shared _cryptoClient
            EncryptResult encryptResult = await _cryptoClient.EncryptAsync(EncryptionAlgorithm.RsaOaep, plaintextBytes);

            return Convert.ToBase64String(encryptResult.Ciphertext);
        }

        public async Task<string> Decrypt(string encryptedText)
        {
            // Convert the Base64 string back to bytes first
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);

            // Decrypt using the shared _cryptoClient
            DecryptResult decryptResult = await _cryptoClient.DecryptAsync(EncryptionAlgorithm.RsaOaep, cipherTextBytes);

            return Encoding.UTF8.GetString(decryptResult.Plaintext);
        }
    }
}
