namespace AsyncJsonClient
{
    public class Request
    {
        public string Operation { get; set; }  // 'create', 'edit', 'delete', 'list'
        public int PokemonId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Sexo { get; set; }  // Nuevo atributo
        public int Poder { get; set; }    // Nuevo atributo
    }
}
