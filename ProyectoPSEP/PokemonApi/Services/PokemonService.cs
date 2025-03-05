using PokemonApi.Models;
using System.Text.Json;

namespace PokemonApi.Services
{
    public class PokemonService
    {
        private readonly string _filePath = "Data/pokemons.json";
        private List<Pokemon> _pokemons;

        public PokemonService()
        {
            _pokemons = LoadFromJson();
        }

        private List<Pokemon> LoadFromJson()
        {
            if (!File.Exists(_filePath))
                return new List<Pokemon>();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Pokemon>>(json) ?? new List<Pokemon>();
        }

        private void SaveToJson()
        {
            var json = JsonSerializer.Serialize(_pokemons, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public List<Pokemon> GetAll() => _pokemons;

        public Pokemon? GetById(long id) => _pokemons.FirstOrDefault(p => p.Id == id);

        public void Add(Pokemon pokemon)
        {
            pokemon.Id = _pokemons.Any() ? _pokemons.Max(p => p.Id) + 1 : 1;
            _pokemons.Add(pokemon);
            SaveToJson();
        }

        public void Update(Pokemon updatedPokemon)
        {
            var pokemon = GetById(updatedPokemon.Id);
            if (pokemon != null)
            {
                pokemon.Nombre = updatedPokemon.Nombre;
                pokemon.Tipo = updatedPokemon.Tipo;
                pokemon.Nivel = updatedPokemon.Nivel;
                pokemon.Estadisticas = updatedPokemon.Estadisticas;
                pokemon.Movimientos = updatedPokemon.Movimientos;
                pokemon.ActualizadoEn = DateTime.UtcNow;
                SaveToJson();
            }
        }

        public void Delete(long id)
        {
            _pokemons.RemoveAll(p => p.Id == id);
            SaveToJson();
        }
    }
}
