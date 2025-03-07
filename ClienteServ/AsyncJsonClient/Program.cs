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

        static async Task Main()
        {
            client.BaseAddress = new Uri("http://localhost:5062/api/pokemon"); // URL de la API REST

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
            HttpResponseMessage response = await client.GetAsync("");
            string json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("\n🔍 Lista de Pokémon:");
            Console.WriteLine(json);
        }

        static async Task BuscarPokemon()
        {
            Console.Write("🔍 Ingrese ID del Pokémon: ");
            string id = Console.ReadLine();
            HttpResponseMessage response = await client.GetAsync($"/{id}");
            string json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("\n📋 Resultado:");
            Console.WriteLine(json);
        }

        static async Task AgregarPokemon()
        {
            Console.Write("📝 Nombre: ");
            string nombre = Console.ReadLine();
            Console.Write("🔥 Tipo: ");
            string tipo = Console.ReadLine();
            Console.Write("💖 HP: ");
            int hp = int.Parse(Console.ReadLine());
            Console.Write("⚔️ Ataque: ");
            int ataque = int.Parse(Console.ReadLine());

            var nuevoPokemon = new
            {
                Name = nombre,
                Type = tipo,
                Hp = hp,           // Campo Hp
                Attack = ataque    // Campo Attack
            };

            var json = JsonSerializer.Serialize(nuevoPokemon);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("", content);

            Console.WriteLine("\n✅ Pokémon agregado con éxito!");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        static async Task ActualizarPokemon()
        {
            Console.Write("✏️ Ingrese ID del Pokémon a actualizar: ");
            string id = Console.ReadLine();
            Console.Write("📝 Nuevo Nombre: ");
            string nombre = Console.ReadLine();
            Console.Write("🔥 Nuevo Tipo: ");
            string tipo = Console.ReadLine();
            Console.Write("💖 Nuevo HP: ");
            int hp = int.Parse(Console.ReadLine());
            Console.Write("⚔️ Nuevo Ataque: ");
            int ataque = int.Parse(Console.ReadLine());

            var pokemonActualizado = new
            {
                Id = int.Parse(id),
                Name = nombre,
                Type = tipo,
                Hp = hp,          // Campo Hp
                Attack = ataque   // Campo Attack
            };

            var json = JsonSerializer.Serialize(pokemonActualizado);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync($"/{id}", content);

            Console.WriteLine("\n✅ Pokémon actualizado!");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        static async Task EliminarPokemon()
        {
            Console.Write("🗑️ Ingrese ID del Pokémon a eliminar: ");
            string id = Console.ReadLine();

            // Asegúrate de que el id es un número válido y forma la URL completa
            string url = $"/api/pokemon/{id}";

            // Realiza la solicitud DELETE a la URL completa
            HttpResponseMessage response = await client.DeleteAsync(url);

            // Verifica el código de estado de la respuesta
            if (response.IsSuccessStatusCode)
                Console.WriteLine("\n✅ Pokémon eliminado!");
            else
                Console.WriteLine($"\n❌ No se pudo eliminar el Pokémon. Código de estado: {response.StatusCode}");
        }

    }
}
