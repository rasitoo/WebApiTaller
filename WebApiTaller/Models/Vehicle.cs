namespace WebApiTaller.Models;

public class Vehicle
{
    public string Id { get; set; }
    public string UserId { get; set; }  // Referencia
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public List<string> Components { get; set; } // IDs de componentes (puedes embebidos o referenciados)

}
