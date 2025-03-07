using Newtonsoft.Json;
using PokemonAPI.Models;

namespace PokemonAPI.Services;

public class PokemonService
{
    private readonly string _filePath = "pokemons.json";
    private readonly object _lock = new(); // 🔒 Bloqueo para evitar accesos simultáneos

    public List<Pokemon> GetPokemons()
    {
        if (!File.Exists(_filePath))
            return new List<Pokemon>();

        var json = File.ReadAllText(_filePath);
        return JsonConvert.DeserializeObject<List<Pokemon>>(json) ?? new List<Pokemon>();
    }

    public void SavePokemons(List<Pokemon> pokemons)
    {
        lock (_lock) // 🔒 Bloqueo para evitar problemas con varios clientes
        {
            var json = JsonConvert.SerializeObject(pokemons, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }

    public Pokemon AddPokemon(Pokemon pokemon)
    {
        lock (_lock) // 🔒 Evita problemas de concurrencia
        {
            var pokemons = GetPokemons();
            pokemon.Id = pokemons.Count > 0 ? pokemons.Max(p => p.Id) + 1 : 1; // Generar ID único
            pokemons.Add(pokemon);
            SavePokemons(pokemons);
            return pokemon;
        }
    }
}
