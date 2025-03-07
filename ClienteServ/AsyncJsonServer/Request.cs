namespace AsyncJsonServer
{
    public class Request
    {
        public string Operation { get; set; } 
        public int PokemonId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Hp { get; set; }  // Cambiado de Sexo a Hp
        public int Poder { get; set; }   
    }
}
