using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace PokemonClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Client.StartAsync();
        }
    }

    public class Client
    {
        private static readonly RSACryptoServiceProvider rsa = new(2048);

        public static async Task StartAsync()
        {
            try
            {
                Console.Write("Ingrese la dirección IP del servidor (default: 127.0.0.1): ");
                string? ipInput = Console.ReadLine();
                IPAddress ipAddress = !string.IsNullOrWhiteSpace(ipInput) && ValidateIpAddress(ipInput)
                                      ? IPAddress.Parse(ipInput)
                                      : IPAddress.Loopback;

                IPEndPoint remoteEP = new(ipAddress, 11000);
                using Socket sender = new(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                await sender.ConnectAsync(remoteEP);
                Console.WriteLine("Conectado al servidor principal.");

                byte[] buffer = new byte[4096];
                int bytesRec = await sender.ReceiveAsync(buffer, SocketFlags.None);
                string publicKeyXml = Encoding.UTF8.GetString(buffer, 0, bytesRec);
                rsa.FromXmlString(publicKeyXml);

                while (true)
                {
                    Console.WriteLine("Seleccione una opción:");
                    Console.WriteLine("1. Agregar un nuevo Pokémon");
                    Console.WriteLine("2. Ver todos los Pokémon");
                    Console.WriteLine("3. Ver un Pokémon específico");
                    Console.WriteLine("4. Modificar un Pokémon");
                    Console.WriteLine("5. Liberar un Pokémon");
                    Console.WriteLine("6. Salir");

                    string? choice = Console.ReadLine();
                    if (choice == "6") break;

                    string operationMessage = choice switch
                    {
                        "1" => await AddPokemonAsync(),
                        "2" => "Ver todos los Pokémon",
                        "3" => await ViewPokemonAsync(),
                        "4" => await ModifyPokemonAsync(),
                        "5" => await DeletePokemonAsync(),
                        _ => "Opción no válida"
                    };

                    if (operationMessage == "Opción no válida") continue;

                    string encryptedMessage = EncryptMessage(operationMessage);
                    byte[] encryptedBytes = Encoding.UTF8.GetBytes(encryptedMessage);
                    await sender.SendAsync(encryptedBytes, SocketFlags.None);

                    bytesRec = await sender.ReceiveAsync(buffer, SocketFlags.None);
                    string encryptedResponse = Encoding.UTF8.GetString(buffer, 0, bytesRec);
                    Console.WriteLine("Servidor principal: " + encryptedResponse);
                }

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
        }

        static bool ValidateIpAddress(string ipInput) => IPAddress.TryParse(ipInput, out _);

        private static string EncryptMessage(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] encryptedBytes = rsa.Encrypt(messageBytes, false);
            return Convert.ToBase64String(encryptedBytes);
        }

        // Agregar Pokémon con validaciones
        private static async Task<string> AddPokemonAsync()
        {
            Console.WriteLine("Ingrese los datos del Pokémon:");

            string nombre = await PromptForStringAsync("Nombre");
            string tipo = await PromptForStringAsync("Tipo");

            int nivel = await PromptForIntAsync("Nivel");
            double poder = await PromptForDoubleAsync("Poder");

            // Aquí se enviaría la información al servidor
            var pokemon = new Pokemon { Nombre = nombre, Tipo = tipo, Nivel = nivel, Poder = poder };
            string encryptedMessage = EncryptMessage(JsonSerializer.Serialize(pokemon));  // Encriptamos los datos antes de enviarlos
            byte[] encryptedBytes = Encoding.UTF8.GetBytes(encryptedMessage);
            return $"Se ha agregado un Pokémon: {nombre} ({tipo}), Nivel: {nivel}, Poder: {poder}";
        }

        // Ver un Pokémon específico con validación de ID
        private static async Task<string> ViewPokemonAsync()
        {
            int id = await PromptForIntAsync("ID del Pokémon");

            // Aquí se enviaría el ID al servidor para obtener el Pokémon
            return $"Ver Pokémon con ID: {id}";
        }

        // Modificar Pokémon con validación de ID
        private static async Task<string> ModifyPokemonAsync()
        {
            int id = await PromptForIntAsync("ID del Pokémon a modificar");

            string nombre = await PromptForStringAsync("Nuevo Nombre");
            string tipo = await PromptForStringAsync("Nuevo Tipo");
            int nivel = await PromptForIntAsync("Nuevo Nivel");
            double poder = await PromptForDoubleAsync("Nuevo Poder");

            var pokemon = new Pokemon { Id = id, Nombre = nombre, Tipo = tipo, Nivel = nivel, Poder = poder };
            string encryptedMessage = EncryptMessage(JsonSerializer.Serialize(pokemon));  // Encriptamos los datos antes de enviarlos
            byte[] encryptedBytes = Encoding.UTF8.GetBytes(encryptedMessage);
            return $"Se ha modificado el Pokémon con ID: {id}, Nuevo Nombre: {nombre}, Nuevo Tipo: {tipo}, Nivel: {nivel}, Poder: {poder}";
        }

        // Eliminar un Pokémon con validación de ID
        private static async Task<string> DeletePokemonAsync()
        {
            int id = await PromptForIntAsync("ID del Pokémon a eliminar");

            // Enviar al servidor para que elimine el Pokémon
            string operationMessage = $"Eliminar Pokémon con ID: {id}";
            string encryptedMessage = EncryptMessage(operationMessage);
            byte[] encryptedBytes = Encoding.UTF8.GetBytes(encryptedMessage);
            return $"Se ha solicitado la eliminación del Pokémon con ID: {id}";
        }

        // Función para pedir un string con validación
        private static async Task<string> PromptForStringAsync(string field)
        {
            string input;
            do
            {
                Console.Write($"{field}: ");
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine($"{field} no puede estar vacío. Inténtelo de nuevo.");
                }
            } while (string.IsNullOrWhiteSpace(input));
            return input!;
        }

        // Función para pedir un int con validación
        private static async Task<int> PromptForIntAsync(string field)
        {
            int result;
            do
            {
                Console.Write($"{field}: ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out result))
                {
                    break;
                }
                else
                {
                    Console.WriteLine($"El valor de {field} debe ser un número entero. Inténtelo de nuevo.");
                }
            } while (true);
            return result;
        }

        // Función para pedir un double con validación
        private static async Task<double> PromptForDoubleAsync(string field)
        {
            double result;
            do
            {
                Console.Write($"{field}: ");
                string input = Console.ReadLine();
                if (double.TryParse(input, out result))
                {
                    break;
                }
                else
                {
                    Console.WriteLine($"El valor de {field} debe ser un número válido. Inténtelo de nuevo.");
                }
            } while (true);
            return result;
        }
    }

    public class Pokemon
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Tipo { get; set; }
        public int Nivel { get; set; }
        public double Poder { get; set; }
    }
}
