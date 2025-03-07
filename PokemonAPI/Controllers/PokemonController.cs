using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PokemonApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private const string JsonFilePath = "pokemons.json";

        private List<Pokemon> LoadPokemons()
        {
            if (!System.IO.File.Exists(JsonFilePath))
                return new List<Pokemon>();

            var json = System.IO.File.ReadAllText(JsonFilePath);
            return JsonSerializer.Deserialize<List<Pokemon>>(json);
        }

        private void SavePokemons(List<Pokemon> pokemons)
        {
            var json = JsonSerializer.Serialize(pokemons, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(JsonFilePath, json);
        }

        [HttpGet]
        public IActionResult GetAllPokemons()
        {
            var pokemons = LoadPokemons();
            return Ok(pokemons);
        }

        [HttpGet("{id}")]
        public IActionResult GetPokemonById(int id)
        {
            var pokemons = LoadPokemons();
            var pokemon = pokemons.FirstOrDefault(p => p.Id == id);
            if (pokemon == null)
            {
                return NotFound();
            }
            return Ok(pokemon);
        }

        [HttpPost]
        public IActionResult CreatePokemon([FromBody] Pokemon newPokemon)
        {
            var pokemons = LoadPokemons();
            newPokemon.Id = pokemons.Max(p => p.Id) + 1;
            pokemons.Add(newPokemon);
            SavePokemons(pokemons);
            return CreatedAtAction(nameof(GetPokemonById), new { id = newPokemon.Id }, newPokemon);
        }

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
            pokemon.Attack = updatedPokemon.Attack;
            SavePokemons(pokemons);

            return Ok(pokemon);
        }

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

    public class Pokemon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Hp { get; set; }
        public int Attack { get; set; }
    }
}
