using ReceitasApi.Model;

namespace ReceitasApi.Data;

public class Alergias
{
    public int Id { get; set; }
    public string Nome { get; set; }

    public int ReceitaId { get; set; }
}