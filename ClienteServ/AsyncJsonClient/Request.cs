namespace AsyncJsonClient
{
    public class Request
    {
        public string Operation { get; set; }  // 'create', 'edit', 'delete', 'list'
        public int PokemonId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Hp { get; set; }  // Tipo correcto: int
        public int Attack { get; set; }  // Nuevo campo Attack
    }
}
