using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PokemonApi.Models;

namespace PokemonApi.Services
{
    public class JsonDataService
    {
        private readonly string _filePath;

        public JsonDataService(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<List<Pokemon>> GetPokemonsAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Pokemon>();
            }

            try
            {
                var json = await File.ReadAllTextAsync(_filePath);
                return JsonSerializer.Deserialize<List<Pokemon>>(json) ?? new List<Pokemon>();
            }
            catch (JsonException)
            {
                return new List<Pokemon>();
            }
        }

        public async Task SavePokemonsAsync(List<Pokemon> pokemons)
        {
            var json = JsonSerializer.Serialize(pokemons, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task<int> GetNextIdAsync()
        {
        var pokemons = await GetPokemonsAsync();
        return pokemons.Count == 0 ? 1 : (int)(pokemons.Max(p => p.Id) + 1);
        }

    }
}
