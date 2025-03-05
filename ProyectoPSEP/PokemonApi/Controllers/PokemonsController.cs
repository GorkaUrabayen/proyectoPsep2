using Microsoft.AspNetCore.Mvc;
using PokemonApi.Models;
using PokemonApi.Services;

namespace PokemonApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly PokemonService _pokemonService;

        public PokemonController(PokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Pokemon>> GetAll()
        {
            return Ok(_pokemonService.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<Pokemon> GetById(long id)
        {
            var pokemon = _pokemonService.GetById(id);
            if (pokemon == null)
            {
                return NotFound();
            }
            return Ok(pokemon);
        }

        [HttpPost]
        public ActionResult<Pokemon> Create(Pokemon pokemon)
        {
            _pokemonService.Add(pokemon);
            return CreatedAtAction(nameof(GetById), new { id = pokemon.Id }, pokemon);
        }

        [HttpPut("{id}")]
        public ActionResult Update(long id, Pokemon updatedPokemon)
        {
            var existingPokemon = _pokemonService.GetById(id);
            if (existingPokemon == null)
            {
                return NotFound();
            }

            updatedPokemon.Id = id;
            _pokemonService.Update(updatedPokemon);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(long id)
        {
            var existingPokemon = _pokemonService.GetById(id);
            if (existingPokemon == null)
            {
                return NotFound();
            }

            _pokemonService.Delete(id);
            return NoContent();
        }
    }
}
