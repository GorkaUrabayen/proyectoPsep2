namespace PokemonAPI.Models;

public class Pokemon
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Sexo { get; set; }  // Nuevo atributo (Macho, Hembra, Desconocido)
    public int Poder { get; set; }    // Nuevo atributo (Nivel de poder)
}
