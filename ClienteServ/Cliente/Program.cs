using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace ClienteServ.Cliente
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
        public static async Task StartAsync()
        {
            try
            {
                Console.Write("Ingrese la dirección IP del servidor (default: 127.0.0.1): ");
                string? ipInput = Console.ReadLine();
                IPAddress ipAddress = !string.IsNullOrWhiteSpace(ipInput) && ValidateIpAddress(ipInput) 
                                      ? IPAddress.Parse(ipInput!)  // Usamos el operador '!' para asegurar que ipInput no es null.
                                      : IPAddress.Loopback;

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    await sender.ConnectAsync(remoteEP);
                    Console.WriteLine("Socket conectado a {0}", sender.RemoteEndPoint?.ToString());

                    while (true)
                    {
                        Console.Write("Escribe la operación [create, edit, delete, list o 'salir' para desconectar]: ");
                        string? input = Console.ReadLine();

                        if (input == "salir")
                        {
                            Console.WriteLine("Desconectando...");
                            sender.Shutdown(SocketShutdown.Both);
                            sender.Close();
                            break;
                        }

                        // Validar operación
                        if (input == null || (input != "create" && input != "edit" && input != "delete" && input != "list"))
                        {
                            Console.WriteLine("Operación inválida. Por favor, usa 'create', 'edit', 'delete' o 'list'.");
                            continue;
                        }

                        if (input == "list")
                        {
                            // Enviar solicitud para listar todos los Pokémon
                            var requestList = new Request { Operation = "list" };
                            string requestJsonList = JsonConvert.SerializeObject(requestList);
                            byte[] msgList = Encoding.UTF8.GetBytes(requestJsonList);

                            await sender.SendAsync(new ArraySegment<byte>(msgList), SocketFlags.None);

                            byte[] responseBytesList = new byte[1024];
                            int bytesRecList = await sender.ReceiveAsync(new ArraySegment<byte>(responseBytesList), SocketFlags.None);
                            string responseMessageList = Encoding.UTF8.GetString(responseBytesList, 0, bytesRecList);

                            var responseDataList = JsonConvert.DeserializeObject<Response>(responseMessageList);
                            Console.WriteLine("Lista de Pokémon:");
                            foreach (var p in responseDataList?.PokemonTeam ?? new List<Pokemon>())
                            {
                                Console.WriteLine($"{p.Id}: {p.Name} - {p.Type} - {p.Sexo} - Poder: {p.Poder}");
                            }
                            continue;
                        }

                        // Validar los detalles del Pokémon
                        Console.Write("Ingrese los detalles del Pokémon (ID, Nombre, Tipo, Sexo, Poder): ");
                        string? pokemonDetails = Console.ReadLine();
                        var details = pokemonDetails?.Split(',');

                        if (details == null || details.Length != 5)
                        {
                            Console.WriteLine("Formato incorrecto. Asegúrate de proporcionar 5 detalles separados por coma.");
                            continue;
                        }

                        if (!int.TryParse(details[0], out int id) || id <= 0)
                        {
                            Console.WriteLine("El ID debe ser un número entero positivo.");
                            continue;
                        }

                        if (!int.TryParse(details[4], out int poder) || poder <= 0)
                        {
                            Console.WriteLine("El poder debe ser un número entero positivo.");
                            continue;
                        }

                        string sexo = details[3].Trim().ToLower();
                        if (sexo != "macho" && sexo != "hembra" && sexo != "desconocido")
                        {
                            Console.WriteLine("Sexo inválido. Debe ser 'macho', 'hembra' o 'desconocido'.");
                            continue;
                        }

                        var request = new Request
                        {
                            Operation = input,
                            Id = id,
                            Name = details[1],
                            Type = details[2],
                            Sexo = details[3],
                            Poder = poder
                        };

                        string requestJson = JsonConvert.SerializeObject(request);
                        byte[] msg = Encoding.UTF8.GetBytes(requestJson);

                        await sender.SendAsync(new ArraySegment<byte>(msg), SocketFlags.None);

                        byte[] responseBytes = new byte[1024];
                        int bytesRec = await sender.ReceiveAsync(new ArraySegment<byte>(responseBytes), SocketFlags.None);
                        string responseMessage = Encoding.UTF8.GetString(responseBytes, 0, bytesRec);

                        var responseData = JsonConvert.DeserializeObject<Response>(responseMessage);
                        Console.WriteLine("Respuesta del servidor: {0}", responseData?.Message);
                        Console.WriteLine("Equipo Pokémon actualizado:");
                        foreach (var p in responseData?.PokemonTeam ?? new List<Pokemon>())
                        {
                            Console.WriteLine($"{p.Id}: {p.Name} - {p.Type} - {p.Sexo} - Poder: {p.Poder}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Excepción inesperada: {0}", e.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static bool ValidateIpAddress(string ipInput)
        {
            return IPAddress.TryParse(ipInput, out _);
        }
    }

    public class Request
    {
        public string Operation { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Sexo { get; set; }
        public int Poder { get; set; }
    }

    public class Response
    {
        public string Message { get; set; }
        public List<Pokemon> PokemonTeam { get; set; }
    }

    public class Pokemon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Sexo { get; set; }
        public int Poder { get; set; }
    }
}
