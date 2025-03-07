using System;
using System.Security.Cryptography;
using System.Text;

namespace AsyncJsonClient.Crypto
{
    public class RSAEncryption
    {
        private RSA _rsa;

        public RSAEncryption()
        {
            _rsa = RSA.Create();
        }

        // Método para exportar la clave pública en Base64
        public string ExportPublicKey()
        {
            byte[] publicKeyBytes = _rsa.ExportRSAPublicKey();
            return Convert.ToBase64String(publicKeyBytes);
        }

        // Método para exportar la clave privada en Base64
        public string ExportPrivateKey()
        {
            byte[] privateKeyBytes = _rsa.ExportRSAPrivateKey();
            return Convert.ToBase64String(privateKeyBytes);
        }

        // Método para importar la clave pública
        public void ImportPublicKey(string base64PublicKey)
        {
            try
            {
                // Limpiar la cadena Base64 de caracteres no válidos (saltos de línea y espacios)
                base64PublicKey = base64PublicKey.Replace("\r", "").Replace("\n", "").Trim();

                // Verificar si la cadena Base64 es válida antes de intentar convertirla
                if (IsValidBase64(base64PublicKey))
                {
                    byte[] publicKeyBytes = Convert.FromBase64String(base64PublicKey);
                    _rsa.ImportRSAPublicKey(publicKeyBytes, out _);
                    Console.WriteLine("Clave pública importada con éxito.");
                }
                else
                {
                    Console.WriteLine("❌ La clave pública no tiene un formato Base64 válido.");
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"❌ Error al importar la clave pública: {ex.Message}");
            }
        }

        // Método para importar la clave privada
        public void ImportPrivateKey(string base64PrivateKey)
        {
            try
            {
                // Limpiar la cadena Base64 de caracteres no válidos (saltos de línea y espacios)
                base64PrivateKey = base64PrivateKey.Replace("\r", "").Replace("\n", "").Trim();

                // Verificar si la cadena Base64 es válida antes de intentar convertirla
                if (IsValidBase64(base64PrivateKey))
                {
                    byte[] privateKeyBytes = Convert.FromBase64String(base64PrivateKey);
                    _rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
                    Console.WriteLine("Clave privada importada con éxito.");
                }
                else
                {
                    Console.WriteLine("❌ La clave privada no tiene un formato Base64 válido.");
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"❌ Error al importar la clave privada: {ex.Message}");
            }
        }

        // Método para encriptar datos con la clave pública
        public string Encrypt(string plainText, string publicKey)
        {
            ImportPublicKey(publicKey);
            byte[] encrypted = _rsa.Encrypt(Encoding.UTF8.GetBytes(plainText), RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(encrypted);
        }

        // Método para desencriptar datos con la clave privada
        public string Decrypt(string cipherText, string privateKey)
        {
            ImportPrivateKey(privateKey);
            byte[] decrypted = _rsa.Decrypt(Convert.FromBase64String(cipherText), RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(decrypted);
        }

        // Método para verificar si una cadena es un Base64 válido
        private bool IsValidBase64(string base64String)
        {
            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
