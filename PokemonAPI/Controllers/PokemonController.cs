using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using PokemonAPI.Models; // Asegúrate de que este espacio de nombres sea el correcto para el modelo de Pokemon

namespace PokemonApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]  // Indica que la API consume JSON
    [Produces("application/json")]  // Indica que la API produce JSON
    public class PokemonController : ControllerBase
    {
        private const string JsonFilePath = "pokemons.json";

        // Método para cargar los Pokémon desde el archivo JSON
        private List<Pokemon> LoadPokemons()
        {
            if (!System.IO.File.Exists(JsonFilePath))
                return new List<Pokemon>();

            var json = System.IO.File.ReadAllText(JsonFilePath);
            return JsonSerializer.Deserialize<List<Pokemon>>(json);
        }

        // Método para guardar los Pokémon al archivo JSON
        private void SavePokemons(List<Pokemon> pokemons)
        {
            var json = JsonSerializer.Serialize(pokemons, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(JsonFilePath, json);
        }

        // Obtener todos los Pokémon
        [HttpGet]
        public IActionResult GetAllPokemons()
        {
            var pokemons = LoadPokemons();
            return Ok(pokemons);
        }

        // Obtener un Pokémon por su ID
        // Obtener un Pokémon por su ID
        [HttpGet("{id}")]
        public IActionResult GetPokemonById(int id)
        {
            // Cargar los Pokémon desde el archivo JSON
            var pokemons = LoadPokemons();

            // Buscar el Pokémon por ID
            var pokemon = pokemons.FirstOrDefault(p => p.Id == id);

            if (pokemon == null)
            {
                return NotFound(); // Devuelve 404 si no se encuentra el Pokémon
            }

            return Ok(pokemon); // Devuelve el Pokémon si se encuentra
        }



        // Crear un nuevo Pokémon
        [HttpPost]
        public IActionResult CreatePokemon([FromBody] Pokemon newPokemon)
        {
            var pokemons = LoadPokemons();
            newPokemon.Id = pokemons.Max(p => p.Id) + 1; // Asignar un nuevo ID automáticamente
            pokemons.Add(newPokemon);
            SavePokemons(pokemons);
            return CreatedAtAction(nameof(GetPokemonById), new { id = newPokemon.Id }, newPokemon);
        }

        // Actualizar los detalles de un Pokémon existente
        [HttpPut("{id}")]
        public IActionResult UpdatePokemon(int id, [FromBody] Pokemon updatedPokemon)
        {
            var pokemons = LoadPokemons();
            var pokemon = pokemons.FirstOrDefault(p => p.Id == id);
            if (pokemon == null)
            {
                return NotFound();
            }

            pokemon.Name = updatedPokemon.Name;
            pokemon.Type = updatedPokemon.Type;
            pokemon.Hp = updatedPokemon.Hp;
            pokemon.Poder = updatedPokemon.Poder;
            SavePokemons(pokemons);

            return Ok(pokemon);
        }


        // Eliminar un Pokémon por su ID
        [HttpDelete("{id}")]
        public IActionResult DeletePokemon(int id)
        {
            var pokemons = LoadPokemons();
            var pokemon = pokemons.FirstOrDefault(p => p.Id == id);
            if (pokemon == null)
            {
                return NotFound();
            }

            pokemons.Remove(pokemon);
            SavePokemons(pokemons);

            return NoContent();
        }
    }
}
