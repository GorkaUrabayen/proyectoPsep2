namespace PokemonAPI.Models;

public class Pokemon
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public int Hp { get; set; }  // Cambiado de Sexo a Hp
    public int Poder { get; set; }    // Nuevo atributo (Nivel de poder)
}
