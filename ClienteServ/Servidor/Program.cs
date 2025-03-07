using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClienteServ.Servidor
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
        private static ConcurrentDictionary<string, ClientState> clientStates = new();
        private const int Port = 11000;

        public static async Task StartAsync()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            Console.WriteLine("Servidor iniciado en el puerto {0}", Port);

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }

        private static async Task HandleClientAsync(TcpClient client)
        {
            string clientId = Guid.NewGuid().ToString();
            clientStates[clientId] = new ClientState();

            Console.WriteLine("Cliente conectado: {0}", clientId);

            using NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (client.Connected)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Recibido de {0}: {1}", clientId, request);

                    var requestData = JsonConvert.DeserializeObject<Request>(request);
                    var responseData = ProcessRequest(clientId, requestData);

                    string responseJson = JsonConvert.SerializeObject(responseData);
                    byte[] responseBytes = Encoding.UTF8.GetBytes(responseJson);
                    await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error procesando el mensaje: {0}", ex.Message);
                }
            }

            Console.WriteLine("Cliente desconectado: {0}", clientId);
            clientStates.TryRemove(clientId, out _);
        }

        private static Response ProcessRequest(string clientId, Request request)
        {
            var clientState = clientStates[clientId];

            // Validación de la operación
            if (request.Operation == "list")
            {
                return new Response
                {
                    Message = "Lista de Pokémon",
                    PokemonTeam = clientState.PokemonTeam
                };
            }

            if (request.Operation != "create" && request.Operation != "edit" && request.Operation != "delete")
            {
                return new Response
                {
                    Message = "Operación inválida. Las operaciones válidas son 'create', 'edit', 'delete' o 'list'.",
                    PokemonTeam = clientState.PokemonTeam
                };
            }

            switch (request.Operation)
            {
                case "create":
                    // Validar si ya existe un Pokémon con el mismo ID
                    if (clientState.PokemonTeam.Any(p => p.Id == request.Id))
                    {
                        return new Response
                        {
                            Message = $"Ya existe un Pokémon con el ID {request.Id}.",
                            PokemonTeam = clientState.PokemonTeam
                        };
                    }
                    var newPokemon = new Pokemon
                    {
                        Id = request.Id,
                        Name = request.Name,
                        Type = request.Type,
                        Sexo = request.Sexo,
                        Poder = request.Poder
                    };
                    clientState.PokemonTeam.Add(newPokemon);
                    break;

                case "edit":
                    var pokemonToEdit = clientState.PokemonTeam.FirstOrDefault(p => p.Id == request.Id);
                    if (pokemonToEdit == null)
                    {
                        return new Response
                        {
                            Message = $"No se encontró un Pokémon con el ID {request.Id}.",
                            PokemonTeam = clientState.PokemonTeam
                        };
                    }

                    pokemonToEdit.Name = request.Name;
                    pokemonToEdit.Type = request.Type;
                    pokemonToEdit.Sexo = request.Sexo;
                    pokemonToEdit.Poder = request.Poder;
                    break;

                case "delete":
                    var pokemonToDelete = clientState.PokemonTeam.FirstOrDefault(p => p.Id == request.Id);
                    if (pokemonToDelete == null)
                    {
                        return new Response
                        {
                            Message = $"No se encontró un Pokémon con el ID {request.Id}.",
                            PokemonTeam = clientState.PokemonTeam
                        };
                    }

                    clientState.PokemonTeam.Remove(pokemonToDelete);
                    break;
            }

            return new Response
            {
                Message = $"Operación '{request.Operation}' realizada con éxito.",
                PokemonTeam = clientState.PokemonTeam
            };
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

    public class ClientState
    {
        public List<Pokemon> PokemonTeam { get; set; } = new List<Pokemon>();
    }
}
