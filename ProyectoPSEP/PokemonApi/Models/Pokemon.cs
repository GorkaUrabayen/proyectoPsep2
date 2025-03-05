namespace PokemonApi.Models
{
    public class Pokemon
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Tipo { get; set; }
        public int Nivel { get; set; }
        public double Poder { get; set; }
    }
}
