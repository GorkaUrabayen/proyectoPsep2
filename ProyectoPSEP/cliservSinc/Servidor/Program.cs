using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PokemonServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Server.StartAsync();
        }
    }

    public class Server
    {
        private const int Port = 11000;
        private static readonly RSACryptoServiceProvider rsa = new(2048);

        public static async Task StartAsync()
        {
            Console.WriteLine("Servidor iniciado en el puerto {0}", Port);
            TcpListener listener = new(IPAddress.Any, Port);
            listener.Start();

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }

        private static async Task HandleClientAsync(TcpClient client)
        {
            await using NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[8192];
            Console.WriteLine("Cliente conectado");

            string publicKey = rsa.ToXmlString(false);
            byte[] publicKeyBytes = Encoding.UTF8.GetBytes(publicKey);
            await stream.WriteAsync(publicKeyBytes, 0, publicKeyBytes.Length);

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string encryptedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string decryptedMessage = DecryptMessage(encryptedMessage);
                Console.WriteLine($"Mensaje recibido (descifrado): {decryptedMessage}");

                string response = "Operación procesada correctamente.";
                string encryptedResponse = EncryptMessage(response);
                byte[] encryptedResponseBytes = Encoding.UTF8.GetBytes(encryptedResponse);
                await stream.WriteAsync(encryptedResponseBytes, 0, encryptedResponseBytes.Length);
            }

            client.Close();
            Console.WriteLine("Cliente desconectado.");
        }

        private static string DecryptMessage(string encryptedMessage)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedMessage);
            byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, false);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        private static string EncryptMessage(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] encryptedBytes = rsa.Encrypt(messageBytes, false);
            return Convert.ToBase64String(encryptedBytes);
        }
    }
}
