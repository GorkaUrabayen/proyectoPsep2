using Microsoft.AspNetCore.Mvc;
using PokemonApi.Models;
using PokemonApi.Services;
using System.Text.Json;

namespace PokemonApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonsController : ControllerBase
    {
        private static List<Pokemon> _pokemons = new List<Pokemon>
        {
            new Pokemon { Id = 1, Nombre = "Pikachu", Tipo = "Eléctrico", Nivel = 15, Poder = 55.0 },
            new Pokemon { Id = 2, Nombre = "Bulbasaur", Tipo = "Planta", Nivel = 12, Poder = 49.0 }
        };

        private readonly CipherService _cipherService;
        private const string key = "miClaveSecreta";

        public PokemonsController(CipherService cipherService)
        {
            _cipherService = cipherService;
        }

        // 🔹 Obtener todos los Pokémon (SIN cifrado)
        [HttpGet]
        public ActionResult<List<Pokemon>> Get()
        {
            return Ok(_pokemons);
        }

        // 🔹 Obtener un Pokémon por ID (SIN cifrado)
        [HttpGet("{id}")]
        public ActionResult<Pokemon> GetById(int id)
        {
            var pokemon = _pokemons.FirstOrDefault(p => p.Id == id);
            if (pokemon == null) return NotFound();
            return Ok(pokemon);
        }

        // 🔹 Crear un nuevo Pokémon (Datos cifrados en la solicitud)
        [HttpPost]
        public ActionResult Post([FromBody] EncryptedData encryptedData)
        {
            var decryptedJson = _cipherService.Decrypt(encryptedData.Data, key);
            var pokemon = JsonSerializer.Deserialize<Pokemon>(decryptedJson);

            if (pokemon == null) return BadRequest(new { error = "Datos incorrectos" });

            _pokemons.Add(pokemon);
            return CreatedAtAction(nameof(Get), new { id = pokemon.Id }, pokemon);
        }

        // 🔹 Actualizar un Pokémon (Datos cifrados en la solicitud)
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] EncryptedData encryptedData)
        {
            var decryptedJson = _cipherService.Decrypt(encryptedData.Data, key);
            var pokemon = JsonSerializer.Deserialize<Pokemon>(decryptedJson);

            if (pokemon == null) return BadRequest(new { error = "Datos incorrectos" });

            var existingPokemon = _pokemons.FirstOrDefault(p => p.Id == id);
            if (existingPokemon == null) return NotFound();

            existingPokemon.Nombre = pokemon.Nombre;
            existingPokemon.Tipo = pokemon.Tipo;
            existingPokemon.Nivel = pokemon.Nivel;
            existingPokemon.Poder = pokemon.Poder;

            return NoContent();
        }

        // 🔹 Eliminar un Pokémon
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var pokemon = _pokemons.FirstOrDefault(p => p.Id == id);
            if (pokemon == null) return NotFound();

            _pokemons.Remove(pokemon);
            return NoContent();
        }
    }

    // Clase para manejar los datos cifrados
    public class EncryptedData
    {
        public string Data { get; set; } = string.Empty;
    }
}
