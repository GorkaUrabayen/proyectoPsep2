namespace PokemonApi.Models
{
    public class Pokemon
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public int Nivel { get; set; }
        public Estadisticas Estadisticas { get; set; }
        public List<string> Movimientos { get; set; }
        public DateTime CreadoEn { get; set; }
        public DateTime ActualizadoEn { get; set; }
    }

    public class Estadisticas
    {
        public int HP { get; set; }
        public int Ataque { get; set; }
        public int Defensa { get; set; }
    }
}
