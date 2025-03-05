using System;
using System.Text;

namespace PokemonApi.Services
{
    public class CipherService
    {
        // 🔹 Cifrado con Base64
        public string Encrypt(string plainText, string key)
        {
            StringBuilder cipherText = new StringBuilder();
            for (int i = 0; i < plainText.Length; i++)
            {
                cipherText.Append((char)(plainText[i] ^ key[i % key.Length]));
            }
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(cipherText.ToString()));
        }

        // 🔹 Descifrado desde Base64
        public string Decrypt(string cipherText, string key)
        {
            var decodedBytes = Convert.FromBase64String(cipherText);
            string decodedText = Encoding.UTF8.GetString(decodedBytes);

            StringBuilder plainText = new StringBuilder();
            for (int i = 0; i < decodedText.Length; i++)
            {
                plainText.Append((char)(decodedText[i] ^ key[i % key.Length]));
            }
            return plainText.ToString();
        }
    }
}
