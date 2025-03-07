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
            try
            {
                HttpResponseMessage response = await client.GetAsync("");
                response.EnsureSuccessStatusCode(); // Asegurarse de que la respuesta sea exitosa

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

            // Validar que el ID sea un número válido
            if (!int.TryParse(idInput, out int id))
            {
                Console.WriteLine("❌ El ID ingresado no es válido. Por favor ingrese un número.");
                return;
            }

            try
            {
                HttpResponseMessage response = await client.GetAsync($"{client.BaseAddress}/{id}");
                response.EnsureSuccessStatusCode(); // Asegurarse de que la respuesta sea exitosa

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
                response.EnsureSuccessStatusCode(); // Asegurarse de que la respuesta sea exitosa

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

            // Validar que el ID sea un número válido
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

            Console.Write("🔥 Nuevo Tipo: 1");
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
                Id = id,  // Asegúrate de enviar el ID correcto
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
                response.EnsureSuccessStatusCode(); // Asegúrate de que la respuesta sea exitosa

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

            // Validar que el ID sea un número válido
            if (!int.TryParse(idInput, out int id))
            {
                Console.WriteLine("❌ El ID ingresado no es válido. Por favor ingrese un número.");
                return;
            }

            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"/{id}"); // Solicitud DELETE para eliminar Pokémon
                response.EnsureSuccessStatusCode(); // Asegurarse de que la respuesta sea exitosa

                Console.WriteLine("\n✅ Pokémon eliminado!");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"❌ Error al eliminar Pokémon: {e.Message}");
            }
        }
    }
}
