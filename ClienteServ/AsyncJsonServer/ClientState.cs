namespace AsyncJsonServer
{
    public class ClientState
    {
        // Lista de Pokémon del cliente
        public List<Pokemon> PokemonTeam { get; set; } = new List<Pokemon>();

        // Otros datos relacionados con el cliente (por ejemplo, último Pokémon seleccionado, etc.)
        public string LastSelectedPokemon { get; set; }
        
        // Información adicional del cliente (puedes agregar más propiedades si es necesario)
        public DateTime ConnectionTime { get; set; }
    }
}
