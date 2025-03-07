using AsyncJsonClient.Crypto;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AsyncJsonClient.Cliente
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly RSAEncryption rsaEncryption = new RSAEncryption();

        private static string clientPublicKey;
        private static string clientPrivateKey;
        private static string serverPublicKey = "<insertar clave pública del servidor aquí>"; // Necesitas la clave pública del servidor

        static async Task Main()
        {
            client.BaseAddress = new Uri("http://localhost:5062/api/pokemon");

            // Generar claves RSA para el cliente
            clientPublicKey = rsaEncryption.ExportPublicKey();
            clientPrivateKey = rsaEncryption.ExportPrivateKey();

            // Verificar si las claves generadas son Base64 válidas
            if (!IsValidBase64(clientPublicKey) || !IsValidBase64(clientPrivateKey))
            {
                Console.WriteLine("❌ Error: Las claves generadas no son válidas en formato Base64.");
                return;
            }

            Console.WriteLine("🔑 Claves RSA generadas para el cliente:");
            Console.WriteLine($"📌 Clave pública del cliente: {clientPublicKey}");
            Console.WriteLine($"🔒 Clave privada del cliente: {clientPrivateKey}");

            while (true)
            {
                Console.WriteLine("\n📌 Seleccione una opción:");
                Console.WriteLine("1. Listar Pokémon");
                Console.WriteLine("2. Buscar Pokémon por ID");
                Console.WriteLine("3. Agregar Pokémon");
                Console.WriteLine("4. Actualizar Pokémon");
                Console.WriteLine("5. Eliminar Pokémon");
                Console.WriteLine("6. Salir");

                string opcion = Console.ReadLine();
                switch (opcion)
                {
                    case "1":
                        await ListarPokemones();
                        break;
                    case "2":
                        await BuscarPokemon();
                        break;
                    case "3":
                        await AgregarPokemon();
                        break;
                    case "4":
                        await ActualizarPokemon();
                        break;
                    case "5":
                        await EliminarPokemon();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("❌ Opción no válida. Intente de nuevo.");
                        break;
                }
            }
        }

        static async Task ListarPokemones()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("");
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                Console.WriteLine("\n🔍 Lista de Pokémon:");
                Console.WriteLine(json);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"❌ Error al listar los Pokémon: {e.Message}");
            }
        }

        static async Task BuscarPokemon()
        {
            Console.Write("🔍 Ingrese ID del Pokémon: ");
            string idInput = Console.ReadLine();

            if (!int.TryParse(idInput, out int id))
            {
                Console.WriteLine("❌ El ID ingresado no es válido. Por favor ingrese un número.");
                return;
            }

            try
            {
                HttpResponseMessage response = await client.GetAsync($"{client.BaseAddress}/{id}");
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                Console.WriteLine("\n📋 Resultado:");
                Console.WriteLine(json);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"❌ Error al buscar Pokémon: {e.Message}");
            }
        }

        static async Task AgregarPokemon()
        {
            Console.Write("📝 Nombre: ");
            string nombre = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                Console.WriteLine("❌ El nombre no puede estar vacío.");
                return;
            }

            Console.Write("🔥 Tipo: ");
            string tipo = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(tipo))
            {
                Console.WriteLine("❌ El tipo no puede estar vacío.");
                return;
            }

            Console.Write("💖 HP: ");
            if (!int.TryParse(Console.ReadLine(), out int hp) || hp < 0)
            {
                Console.WriteLine("❌ El HP debe ser un número entero positivo.");
                return;
            }

            Console.Write("⚔️ Ataque: ");
            if (!int.TryParse(Console.ReadLine(), out int ataque) || ataque < 0)
            {
                Console.WriteLine("❌ El Ataque debe ser un número entero positivo.");
                return;
            }

            var nuevoPokemon = new
            {
                Name = nombre,
                Type = tipo,
                Hp = hp,
                Attack = ataque
            };

            var json = JsonSerializer.Serialize(nuevoPokemon);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync("", content);
                response.EnsureSuccessStatusCode();

                Console.WriteLine("\n✅ Pokémon agregado con éxito!");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"❌ Error al agregar Pokémon: {e.Message}");
            }
        }

        static async Task ActualizarPokemon()
        {
            Console.Write("✏️ Ingrese ID del Pokémon a actualizar: ");
            string idInput = Console.ReadLine();

            if (!int.TryParse(idInput, out int id))
            {
                Console.WriteLine("❌ El ID ingresado no es válido. Por favor ingrese un número.");
                return;
            }

            Console.Write("📝 Nuevo Nombre: ");
            string nombre = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                Console.WriteLine("❌ El nombre no puede estar vacío.");
                return;
            }

            Console.Write("🔥 Nuevo Tipo: ");
            string tipo = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(tipo))
            {
                Console.WriteLine("❌ El tipo no puede estar vacío.");
                return;
            }

            Console.Write("💖 Nuevo HP: ");
            if (!int.TryParse(Console.ReadLine(), out int hp) || hp < 0)
            {
                Console.WriteLine("❌ El HP debe ser un número entero positivo.");
                return;
            }

            Console.Write("⚔️ Nuevo Ataque: ");
            if (!int.TryParse(Console.ReadLine(), out int ataque) || ataque < 0)
            {
                Console.WriteLine("❌ El Ataque debe ser un número entero positivo.");
                return;
            }

            var pokemonActualizado = new
            {
                Id = id,
                Name = nombre,
                Type = tipo,
                Hp = hp,
                Attack = ataque
            };

            var json = JsonSerializer.Serialize(pokemonActualizado);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PutAsync($"{client.BaseAddress}/{id}", content);
                response.EnsureSuccessStatusCode();

                Console.WriteLine("\n✅ Pokémon actualizado!");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"❌ Error al actualizar Pokémon: {e.Message}");
            }
        }

        static async Task EliminarPokemon()
        {
            Console.Write("🗑️ Ingrese ID del Pokémon a eliminar: ");
            string idInput = Console.ReadLine();

            if (!int.TryParse(idInput, out int id))
            {
                Console.WriteLine("❌ El ID ingresado no es válido. Por favor ingrese un número.");
                return;
            }

            try
            {
                // Enviar la solicitud DELETE directamente con el ID
                HttpResponseMessage response = await client.DeleteAsync($"{client.BaseAddress}/{id}");

                // Verificar si la respuesta fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("\n✅ Pokémon eliminado con éxito!");
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    Console.WriteLine($"❌ Error al eliminar Pokémon. Código de estado: {response.StatusCode}");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"❌ Error al eliminar Pokémon: {e.Message}");
            }
        }


        static bool IsValidBase64(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return false;
            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (FormatException)
            {
                Console.WriteLine("❌ Error: La cadena no es un Base64 válido.");
                return false;
            }
        }
    }
}
