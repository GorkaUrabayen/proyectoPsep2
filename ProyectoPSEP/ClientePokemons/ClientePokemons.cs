using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class ClientePokemons
{
    static async Task Main(string[] args)
    {
        var baseUrl = "https://localhost:5001/api/Pokemon";  // URL de la API
        using var client = new HttpClient();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Menú API REST Pokémon ===");
            Console.WriteLine("1. Mostrar todos los Pokémon");
            Console.WriteLine("2. Mostrar un Pokémon por ID");
            Console.WriteLine("3. Crear un nuevo Pokémon");
            Console.WriteLine("4. Actualizar un Pokémon");
            Console.WriteLine("5. Eliminar un Pokémon por ID");
            Console.WriteLine("6. Salir");
            Console.Write("Elige una opción: ");
            
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    await GetAllPokemon(client, baseUrl);
                    break;
                case "2":
                    Console.Write("Introduce el ID del Pokémon: ");
                    if (long.TryParse(Console.ReadLine(), out long idBuscar))
                    {
                        await GetPokemonById(client, baseUrl, idBuscar);
                    }
                    else
                    {
                        Console.WriteLine("ID inválido.");
                    }
                    break;
                case "3":
                    await CreatePokemon(client, baseUrl);
                    break;
                case "4":
                    await UpdatePokemon(client, baseUrl);
                    break;
                case "5":
                    Console.Write("Introduce el ID del Pokémon a eliminar: ");
                    if (long.TryParse(Console.ReadLine(), out long idEliminar))
                    {
                        await DeletePokemon(client, baseUrl, idEliminar);
                    }
                    else
                    {
                        Console.WriteLine("ID inválido.");
                    }
                    break;
                case "6":
                    Console.WriteLine("Saliendo del programa...");
                    return;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
            
            Console.WriteLine("\nPresiona Enter para continuar...");
            Console.ReadLine();
        }
    }

    static async Task GetAllPokemon(HttpClient client, string url)
    {
        try
        {
            var response = await client.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("\nPokémon obtenidos:");
                Console.WriteLine(content);
            }
            else
            {
                Console.WriteLine($"Error al obtener Pokémon: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static async Task GetPokemonById(HttpClient client, string url, long id)
    {
        try
        {
            var response = await client.GetAsync($"{url}/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Pokémon encontrado:\n{content}");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Pokémon con ID {id} no encontrado.");
            }
            else
            {
                Console.WriteLine($"Error al buscar el Pokémon: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static async Task CreatePokemon(HttpClient client, string url)
    {
        try
        {
            Console.Write("Introduce el nombre del Pokémon: ");
            string nombre = Console.ReadLine();

            Console.Write("Introduce el tipo del Pokémon: ");
            string tipo = Console.ReadLine();

            Console.Write("Introduce el nivel del Pokémon: ");
            if (!int.TryParse(Console.ReadLine(), out int nivel))
            {
                Console.WriteLine("Nivel inválido.");
                return;
            }

            Console.WriteLine("=== Estadísticas ===");
            Console.Write("Introduce los puntos de vida (HP): ");
            if (!int.TryParse(Console.ReadLine(), out int hp))
            {
                Console.WriteLine("HP inválido.");
                return;
            }

            Console.Write("Introduce el ataque: ");
            if (!int.TryParse(Console.ReadLine(), out int ataque))
            {
                Console.WriteLine("Ataque inválido.");
                return;
            }

            Console.Write("Introduce la defensa: ");
            if (!int.TryParse(Console.ReadLine(), out int defensa))
            {
                Console.WriteLine("Defensa inválida.");
                return;
            }

            Console.WriteLine("=== Movimientos ===");
            var movimientos = new List<string>();
            while (movimientos.Count < 4)
            {
                Console.Write($"Introduce un movimiento ({movimientos.Count + 1}/4): ");
                string movimiento = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(movimiento))
                {
                    movimientos.Add(movimiento);
                }
            }

            var pokemon = new
            {
                Nombre = nombre,
                Tipo = tipo,
                Nivel = nivel,
                Estadisticas = new
                {
                    HP = hp,
                    Ataque = ataque,
                    Defensa = defensa
                },
                Movimientos = movimientos,
                CreadoEn = DateTime.UtcNow,
                ActualizadoEn = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(pokemon);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Pokémon creado con éxito.");
            }
            else
            {
                Console.WriteLine($"Error al crear el Pokémon: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static async Task UpdatePokemon(HttpClient client, string url)
    {
        try
        {
            Console.Write("Introduce el ID del Pokémon a actualizar: ");
            if (!long.TryParse(Console.ReadLine(), out long id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            Console.Write("Introduce el nuevo nombre del Pokémon: ");
            string nombre = Console.ReadLine();

            Console.Write("Introduce el nuevo tipo del Pokémon: ");
            string tipo = Console.ReadLine();

            Console.Write("Introduce el nuevo nivel del Pokémon: ");
            if (!int.TryParse(Console.ReadLine(), out int nivel))
            {
                Console.WriteLine("Nivel inválido.");
                return;
            }

            var pokemon = new
            {
                Id = id,
                Nombre = nombre,
                Tipo = tipo,
                Nivel = nivel,
                ActualizadoEn = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(pokemon);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{url}/{id}", content);
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Pokémon con ID {id} actualizado correctamente.");
            }
            else
            {
                Console.WriteLine($"Error al actualizar el Pokémon: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static async Task DeletePokemon(HttpClient client, string url, long id)
    {
        try
        {
            var response = await client.DeleteAsync($"{url}/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Pokémon con ID {id} eliminado correctamente.");
            }
            else
            {
                Console.WriteLine($"Error al eliminar el Pokémon: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
